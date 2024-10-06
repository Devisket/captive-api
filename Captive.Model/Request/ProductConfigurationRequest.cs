using Captive.Data.Enums;

namespace Captive.Model.Request
{
    public class ProductConfigurationRequest
    {     
        public string FileName {  get; set; }
        public string ConfigurationData { get; set; }
        public ConfigurationType ConfigurationType { get; set; }
    }
}
