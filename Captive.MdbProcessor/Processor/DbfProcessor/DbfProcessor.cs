using Captive.MdbProcessor.Processor.Interfaces;
using Captive.Model;
using Captive.Model.Dto;
using Captive.Model.Processing.Configurations;
using Microsoft.Extensions.Configuration;
using System.Data.OleDb;

namespace Captive.MdbProcessor.Processor.DbfProcessor
{
    public class DbfProcessor : IProcessor<DbfConfiguration>
    {
        private Dictionary<string, string> columnFields;
        private readonly IConfiguration _configuration;

        public DbfProcessor(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public IEnumerable<CheckOrderDto> Extractfile(OrderfileDto orderFile, DbfConfiguration config)
        {
            List<CheckOrderDto> checkOrders = new List<CheckOrderDto>();

            this.columnFields = config.ToDictionary();

            var rootPath = _configuration["Processing:FileProcessing"];

            if (rootPath == null)
            {
                throw new Exception("There is no FileProcessing directory configuration");
            }

            var fileDirectory = String.Concat(rootPath, orderFile.FilePath);

            var password = config.HasPassword ? $";Jet OLEDB:Database Password={config.Password}" : string.Empty;

            string strConnectionString =
                $"Provider='VfpOleDB.1';Data Source={Path.GetDirectoryName(fileDirectory)}"
                + password +
                ";Mode=Share Exclusive;Persist Security Info=True;";

            // Important part - using mdw file
            //strConnectionString += "Jet OLEDB:System Database=" +
            //        Environment.GetEnvironmentVariable("APPDATA") +
            //        @"\Microsoft\Access\system.mdw";

            using (var connection = new OleDbConnection(strConnectionString))
            {
                try
                {
                    connection.Open();

                    // Create an OleDbCommand to retrieve data from a table
                    string query = $"SELECT * FROM {Path.GetFileNameWithoutExtension(orderFile.FilePath)}";

                    OleDbCommand command = new OleDbCommand(query, connection);

                    // Execute the command and read the results
                    OleDbDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        var fieldValues = Enumerable.Range(0, reader.FieldCount).ToDictionary(key => reader.GetName(key).ToLower(), reader.GetValue);

                        if (fieldValues == null)
                            continue;

                        var checkOrder = new CheckOrderDto
                        {
                            CheckType = GetValue(fieldValues, GetColumnName(FileConfigurationConstants.CHECK_TYPE)) ?? string.Empty,
                            FormType = GetValue(fieldValues, GetColumnName(FileConfigurationConstants.FORM_TYPE)) ?? string.Empty,
                            BRSTN = GetValue(fieldValues, GetColumnName(FileConfigurationConstants.BRSTN), config.IsEncrypted) ?? string.Empty,
                            BranchCode = GetValue(fieldValues, GetColumnName(FileConfigurationConstants.BRANCH_CODE), config.IsEncrypted) ?? string.Empty,
                            AccountNumber = GetValue(fieldValues, GetColumnName(FileConfigurationConstants.ACCOUNT_NUMBER), config.IsEncrypted) ?? string.Empty,
                            AccountName1 = GetValue(fieldValues, GetColumnName(FileConfigurationConstants.ACCOUNT_NAME_1), config.IsEncrypted) ?? string.Empty,
                            AccountName2 = GetValue(fieldValues, GetColumnName(FileConfigurationConstants.ACCOUNT_NAME_2), config.IsEncrypted) ?? string.Empty,
                            Quantity = Convert.ToInt32(GetValue(fieldValues, GetColumnName(FileConfigurationConstants.QUANTITY)) ?? "0"),
                            Concode = GetValue(fieldValues, GetColumnName(FileConfigurationConstants.CONCODE)),
                            DeliverTo = GetValue(fieldValues, GetColumnName(FileConfigurationConstants.DELIVER_TO), config.IsEncrypted),
                            StartingSeries = GetValue(fieldValues, GetColumnName(FileConfigurationConstants.STARTING_SERIAL_NO), config.IsEncrypted),
                            EndingSeries = GetValue(fieldValues, GetColumnName(FileConfigurationConstants.ENDING_SERIAL_NO), config.IsEncrypted),
                        };

                        checkOrder.MainAccountName = String.Format("{0} {1}", checkOrder.AccountName1, checkOrder.AccountName2);

                        checkOrders.Add(checkOrder);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }

            return checkOrders;
        }

        public string GetColumnName(string FieldName) =>
            columnFields.ContainsKey(FieldName) ? columnFields[FieldName] : string.Empty;

        public string? GetValue(Dictionary<string, object> fieldValues, string fieldName, bool decrypt = false)
        {

            if (String.IsNullOrEmpty(fieldName))
                return string.Empty;

            if (fieldValues.TryGetValue(fieldName.ToLower(), out object value))
            {
                var valueString = Convert.ToString(value) ?? string.Empty;

                if (decrypt)
                    valueString = Decrypt(valueString);

                return valueString;
            }
            else
            {
                throw new Exception($"Field name: {fieldName} doesn't exist");
            };
        }

        public string Decrypt(string EncryptedValue)
        {
            EncryptedValue = EncryptedValue.Replace(" ", string.Empty);

            if(string.IsNullOrEmpty(EncryptedValue) || string.IsNullOrWhiteSpace(EncryptedValue))
                return string.Empty;

            string Decrypted = @"Y+ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890 .,&'()*-/""";

            string Encrypted = @"1E[\]^_`abcdefghijklmnopqrstKLMNOPQRSJ:HF@ABCDGI<";

            var encryptArr = Encrypted.ToCharArray();

            if (EncryptedValue.Any(x => !encryptArr.Contains(x)))
                return EncryptedValue;

            var decryptedVal = EncryptedValue.Select(x => Decrypted[Encrypted.IndexOf(x)]).ToArray();

            return new string(decryptedVal);
        }
    }
}
