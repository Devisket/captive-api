using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Captive.Processing.Processor.TextFileProcessor
{
    public class TextFileProcessor : ITextFileProcessor
    {
        //private readonly IConfiguration _configuration;
        //private IDictionary<string, Tuple<int, int>> _fileConfiguration;

        //public TextFileProcessor(IConfiguration configuration)
        //{
        //    _configuration = configuration;
        //    _fileConfiguration = new Dictionary<string, Tuple<int, int>>();
        //}

        //public void OnProcessFile(byte[] file)
        //{
        //    IList<OrderFileData> fileDatas = new List<OrderFileData>();

        //    _fileConfiguration = ExtractConfiguration();

        //    ReadFile(file);
        //}

        //public ICollection<OrderFileData> OnProcessFile(byte[] file, string orderFileConfiguration)
        //{
        //    if (orderFileConfiguration == null)
        //    {
        //        throw new ArgumentNullException(nameof(orderFileConfiguration));
        //    }

        //    _fileConfiguration = ExtractConfiguration(orderFileConfiguration);

        //    var returnData = ReadFile(file);

        //    return returnData;
        //}

        //private ICollection<OrderFileData> ReadFile(byte[] file)
        //{
        //    IList<OrderFileData> fileDatas = new List<OrderFileData>();

        //    using (StreamReader reader = new StreamReader(new MemoryStream(file)))
        //    {
        //        List<string> stringArr = new List<string>();

        //        while (!reader.EndOfStream)
        //        {
        //            var stringVal = reader.ReadLine();

        //            if (string.IsNullOrEmpty(stringVal))
        //            {
        //                continue;
        //            }
        //            stringArr.Add(stringVal);
        //        }

        //        for (int i = 0; i < stringArr.Count; i++)
        //        {
        //            var accNo = GetSubstringValue(stringArr[i], FileConfigurationConstants.ACCOUNT_NUMBER);

        //            var conCodeString = GetSubstringValue(stringArr[i], FileConfigurationConstants.CONCODE);
        //            var conCode = string.IsNullOrWhiteSpace(conCodeString) ? 0 : int.Parse(conCodeString);

        //            var deliverTo = _fileConfiguration[FileConfigurationConstants.DELIVER_TO].Item1 >= stringArr[i].Length ? null
        //                                                      : GetSubstringValue(stringArr[i], FileConfigurationConstants.DELIVER_TO) ?? null;

        //            var accountName = GetSubstringValue(stringArr[i], FileConfigurationConstants.ACCOUNT_NAME);
        //            var concode = string.Empty;

        //            var quantity = int.Parse(GetSubstringValue(stringArr[i], FileConfigurationConstants.QUANTITY));

        //            var existingAccount = fileDatas.Where(x => x.AccountNumber == accNo && x.DeliverTo == deliverTo).FirstOrDefault();

        //            if (quantity > 0 && existingAccount != null)
        //            {
        //                existingAccount.Quantity += quantity;
        //                continue;
        //            }

        //            if (quantity == 0)
        //            {
        //                continue;
        //            }

        //            if (conCode > 0)
        //            {
        //                accountName = string.Empty;
        //                var j = i;
        //                var concodeArr = new List<string>();

        //                for (; j < stringArr.Count; j++)
        //                {
        //                    var succeedingAccNo = GetSubstringValue(stringArr[j], FileConfigurationConstants.ACCOUNT_NUMBER);
        //                    var succeedingConcode = string.IsNullOrWhiteSpace(conCodeString) ? 0 : int.Parse(GetSubstringValue(stringArr[j], FileConfigurationConstants.CONCODE));

        //                    if (succeedingAccNo != accNo ||
        //                        succeedingConcode == 0 ||
        //                        succeedingAccNo == accNo &&
        //                        concodeArr.Any(x => int.Parse(GetSubstringValue(x, FileConfigurationConstants.CONCODE)) == succeedingConcode))
        //                    {
        //                        break;
        //                    }

        //                    concodeArr.Add(stringArr[j]);
        //                }

        //                concodeArr.Sort((a, b) =>
        //                {
        //                    var valA = int.Parse(GetSubstringValue(a, FileConfigurationConstants.CONCODE));
        //                    var valB = int.Parse(GetSubstringValue(a, FileConfigurationConstants.CONCODE));

        //                    return valA > valB ? 1 : -1;
        //                });

        //                concodeArr.ForEach((z) =>
        //                {
        //                    var concodeValue = GetSubstringValue(z, FileConfigurationConstants.ACCOUNT_NAME);

        //                    concode = string.IsNullOrEmpty(concode) ? concodeValue : string.Join(";", concode, concodeValue);
        //                    accountName = string.Join(" ", accountName, concodeValue);
        //                });

        //                i = j - 1;
        //            }

        //            fileDatas.Add(new OrderFileData
        //            {
        //                CheckType = GetSubstringValue(stringArr[i], FileConfigurationConstants.CHECK_TYPE),
        //                BRSTN = GetSubstringValue(stringArr[i], FileConfigurationConstants.BRSTN),
        //                AccountNumber = accNo,
        //                Concode = concode,
        //                AccountName = accountName,
        //                FormType = GetSubstringValue(stringArr[i], FileConfigurationConstants.FORM_TYPE),
        //                Quantity = quantity,
        //                DeliverTo = deliverTo
        //            });
        //        }
        //    }

        //    return fileDatas;
        //}

        //public IDictionary<string, Tuple<int, int>> ExtractConfiguration(string? configurationData = null)
        //{
        //    var jsonString = configurationData ?? _configuration["CaptiveConfiguration"];

        //    if (string.IsNullOrEmpty(jsonString))
        //    {
        //        throw new Exception("Captive configuration is empty!");
        //    }

        //    var returnDictionary = new Dictionary<string, Tuple<int, int>>();

        //    var convertedObj = JsonConvert.DeserializeObject<IDictionary<string, IDictionary<string, int>>>(jsonString);


        //    if (convertedObj == null || convertedObj.Count == 0)
        //    {
        //        throw new Exception("Captive configuration is empty!");
        //    }

        //    foreach (var dict in convertedObj)
        //    {
        //        returnDictionary.Add(dict.Key,
        //            new Tuple<int, int>(
        //                dict.Value.First(x => x.Key == "pos").Value - 1,
        //                dict.Value.First(x => x.Key == "maxChar").Value
        //                ));

        //    }
        //    return returnDictionary;
        //}

        //private string GetSubstringValue(string sourceString, string referenceKey)
        //{
        //    return sourceString.Substring(_fileConfiguration[referenceKey].Item1, _fileConfiguration[referenceKey].Item2);
        //}
    }
}
