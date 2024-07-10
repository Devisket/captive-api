
using Captive.Data.Enums;

namespace Captive.Data.Models
{
    public  class ProductConfiguration
    {
        public Guid Id { get; set; }
        public string FileName {  get; set; }   
   
        public required string ConfigurationData { get; set; }
        public required ConfigurationType ConfigurationType { get; set; }
        public bool isActive { get; set; }

        public required Guid ProductId { get; set; }
        public required Product Product { get; set; }
    }
}
