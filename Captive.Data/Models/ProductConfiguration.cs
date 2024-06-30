
using Captive.Data.Enums;

namespace Captive.Data.Models
{
    public  class ProductConfiguration
    {
        public Guid Id { get; set; }
        public Guid BankId { get; set; }
        public string FileName {  get; set; }   
        public required Guid ProductTypeId { get; set; }  
        public required ProductType ProductType { get; set; }
        public required string ConfigurationData { get; set; }
        public required ConfigurationType ConfigurationType { get; set; }
        public bool isActive { get; set; }
    }
}
