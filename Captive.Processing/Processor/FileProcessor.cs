using Captive.Processing.Processor.Model;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Captive.Processing.Processor
{
    public class FileProcessor
    {
        private readonly IConfiguration _configuration;

        public FileProcessor(IConfiguration configuration) 
        {
            _configuration = configuration;
        }

        public void OnProcessFile(byte[]file) 
        {
            IList<OrderFileData> fileDatas = new List<OrderFileData>();

            var configurationData = ExtractConfiguration();

            if (configurationData == null)
            {
                throw new ArgumentNullException(nameof(configurationData));
            }

            using (StreamReader reader = new StreamReader(new MemoryStream(file)))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    if(String.IsNullOrEmpty(line))
                    {
                        continue;
                    }

                    var accNo = line.Substring(configurationData[FileConfigurationConstants.ACCOUNT_NAME].Item1, configurationData[FileConfigurationConstants.ACCOUNT_NAME].Item2);
                    var conCode = line.Substring(configurationData[FileConfigurationConstants.CONCODE].Item1, configurationData[FileConfigurationConstants.CONCODE].Item2);

                    if (!String.IsNullOrEmpty(conCode))
                    {
                        if(fileDatas.Any(x=>x.AccountNumber == accNo))
                        {

                        }
                    }

                    fileDatas.Add(new OrderFileData
                    {
                        CheckType = line.Substring(configurationData[FileConfigurationConstants.CHECK_TYPE].Item1, configurationData[FileConfigurationConstants.CHECK_TYPE].Item2),
                        BRSTN= line.Substring(configurationData[FileConfigurationConstants.BRSTN].Item1, configurationData[FileConfigurationConstants.BRSTN].Item2),
                        AccountNumber = line.Substring(configurationData[FileConfigurationConstants.ACCOUNT_NUMBER].Item1, configurationData[FileConfigurationConstants.ACCOUNT_NUMBER].Item2),
                        AccountName = accNo,
                        ConCode = line.Substring(configurationData[FileConfigurationConstants.CONCODE].Item1, configurationData[FileConfigurationConstants.CONCODE].Item2),
                        FormType = line.Substring(configurationData[FileConfigurationConstants.FORM_TYPE].Item1, configurationData[FileConfigurationConstants.FORM_TYPE].Item2),
                        Quantity = line.Substring(configurationData[FileConfigurationConstants.QUANTITY].Item1, configurationData[FileConfigurationConstants.QUANTITY].Item2),
                        DeliverTo = configurationData[FileConfigurationConstants.DELIVER_TO].Item1 >=  line.Length  ?  null 
                        : line.Substring(configurationData[FileConfigurationConstants.DELIVER_TO].Item1, configurationData[FileConfigurationConstants.DELIVER_TO].Item2) ?? null,
                    });
                }

                Console.WriteLine(fileDatas);
            }
        }

        public IDictionary<string, Tuple<int,int>>? ExtractConfiguration()
        {
            var jsonString = _configuration["CaptiveConfiguration"];

            if(String.IsNullOrEmpty(jsonString))
            {
                throw new Exception("Captive configuration is empty!");
            }

            var returnDictionary = new Dictionary<string, Tuple<int,int>>();

            var convertedObj = JsonConvert.DeserializeObject<IDictionary<string, IDictionary<string, int>>>(jsonString);


            if(convertedObj == null || convertedObj.Count == 0)
            {
                throw new Exception("Captive configuration is empty!");
            }

            foreach(var dict in convertedObj)
            {
                returnDictionary.Add( dict.Key, 
                    new Tuple<int,int>(
                        (dict.Value.First(x => x.Key == "pos").Value-1), 
                        dict.Value.First(x => x.Key == "maxChar").Value
                        ));

            }
            return returnDictionary;
        }
    }
}
