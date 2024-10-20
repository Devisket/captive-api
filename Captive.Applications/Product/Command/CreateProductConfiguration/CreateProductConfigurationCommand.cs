using Captive.Data.Enums;
using MediatR;

namespace Captive.Applications.Product.Command.CreateProductConfiguration
{
    public class CreateProductConfigurationCommand: IRequest<Unit>
    {
        public Guid? Id { get; set; } 
        public Guid ProductId { get; set; }
        public Guid CheckValidationId { get; set; }

        public string FileName {  get; set; }
        public string ConfigurationData { get; set; }
        public ConfigurationType ConfigurationType { get; set; }
    }
}
