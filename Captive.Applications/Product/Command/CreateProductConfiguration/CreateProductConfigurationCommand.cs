using Captive.Data.Enums;
using MediatR;

namespace Captive.Applications.Product.Command.CreateProductConfiguration
{
    public class CreateProductConfigurationCommand: IRequest<Unit>
    {
        public Guid? Id { get; set; } 
        public Guid ProductId { get; set; }
        public required string FileName {  get; set; } = string.Empty;
        public string ConfigurationData { get; set; } = string.Empty;
        public bool IsChangeFileType { get; set; }
        public string FileType { get; set; } = string.Empty;
        public ConfigurationType ConfigurationType { get; set; }
    }
}
