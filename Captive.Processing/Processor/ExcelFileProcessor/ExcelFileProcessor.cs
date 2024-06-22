using Captive.Processing.Processor.Model;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OfficeOpenXml;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Captive.Processing.Processor.ExcelFileProcessor
{
    public class ExcelFileProcessor : IExcelFileProcessor
    {
        private readonly IConfiguration _configuration;
        private IDictionary<string, Tuple<int, object>> _fileConfiguration;

        public ExcelFileProcessor(IConfiguration configuration)
        {
            _configuration = configuration;
            _fileConfiguration = new Dictionary<string, Tuple<int, object>>();
        }

        public ICollection<OrderFileData> OnProcessFile(byte[] file, string orderFileConfiguration)
        {
            _fileConfiguration = ExtractConfiguration(orderFileConfiguration);

            var fileDatas = ReadFileExcel(file);

            return fileDatas;
        }

        private IList<OrderFileData> ReadFileExcel(byte[] file)
        {
            IList<OrderFileData> fileDatas = new List<OrderFileData>();

            using (MemoryStream ms = new MemoryStream(file))
            using (var package = new ExcelPackage(ms))
            {
                if (package.Workbook.Worksheets.Count == 0)
                    throw new Exception("Empty Worksheet");

                var ws = package.Workbook.Worksheets[0];

                var start = ws.Dimension.Start;
                var ending = ws.Dimension.End;

                for(int i = start.Row;  i <= ending.Row; i++)
                {
                    if (i == start.Row)
                        continue;
                    var checkType = GetValue(ws, i, FileConfigurationConstants.CHECK_TYPE);
                    var brstn = GetValue(ws, i, FileConfigurationConstants.BRSTN);
                    var accountNo = GetValue(ws, i, FileConfigurationConstants.ACCOUNT_NUMBER);
                    var accountName = GetValue(ws, i, FileConfigurationConstants.ACCOUNT_NAME);
                    var formType = GetValue(ws, i, FileConfigurationConstants.FORM_TYPE);
                    var quantity = GetValue(ws, i, FileConfigurationConstants.QUANTITY);
                    var deliveryBrstn = GetValue(ws, i, FileConfigurationConstants.DELIVER_TO);
                    var startingSerialNo = GetValue(ws, i, FileConfigurationConstants.STARTING_SERIAL_NO);

                    fileDatas.Add(new OrderFileData
                    {
                        AccountName = accountName ?? string.Empty,
                        AccountNumber = accountNo,
                        BRSTN = brstn,
                        CheckType = checkType,
                        FormType = formType,
                        DeliverTo = deliveryBrstn,
                        Quantity = int.Parse(quantity),
                        StartingSeries = startingSerialNo ?? string.Empty,
                    });
                }
            }

            return fileDatas;
        }
        public IDictionary<string, Tuple<int, object>> ExtractConfiguration(string? configurationData = null)
        {
            var jsonString = configurationData ?? _configuration["CaptiveConfiguration"];

            if (string.IsNullOrEmpty(jsonString))
            {
                throw new Exception("Captive configuration is empty!");
            }

            var returnDictionary = new Dictionary<string, Tuple<int, object>>();

            var convertedObj = JsonConvert.DeserializeObject<IDictionary<string, IDictionary<string, object>>>(jsonString);


            if (convertedObj == null || convertedObj.Count == 0)
            {
                throw new Exception("Captive configuration is empty!");
            }

            foreach (var dict in convertedObj)
            {
                var positionValue =  int.Parse(dict.Value.First(x => x.Key == "pos").Value.ToString() ?? "0");
                var maxChar = int.Parse(dict.Value.First(x => x.Key == "maxChar").Value.ToString() ?? "0");
                object? defaultValue = null;

                if (dict.Value.Any(x => x.Key == "value"))
                    defaultValue = dict.Value.First(x => x.Key == "value").Value;


                if (positionValue == 0  && defaultValue != null)
                {
                    returnDictionary.Add(dict.Key, new Tuple<int, object>(
                        0,
                        defaultValue
                        ));
                }
                else if(positionValue > 0)
                {
                    returnDictionary.Add(dict.Key, new Tuple<int, object>(
                        positionValue,
                        maxChar
                        ));
                }
            }
            return returnDictionary;
        }

        public string GetValue(ExcelWorksheet ws, int row, string Key)
        {
            if (!_fileConfiguration.Any(x => x.Key == Key))
                return string.Empty;
            
            var configData = _fileConfiguration.First(x => x.Key == Key).Value;

            if(configData == null)
            {
                return string.Empty;
            }
            
            if(configData.Item1 == 0 )
            {
                return configData.Item2.ToString() ?? string.Empty;
            }
            else
            {
                return ws.Cells[row, configData.Item1].Value.ToString() ?? string.Empty;
            }
        }

    }
}
