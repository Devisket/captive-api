using Captive.Data.Enums;
using Captive.Data.Models;

namespace Captive.Model.Dto
{
    public class ProductConfigurationDto
    {
        public Guid? Id { get; set; }
        public Guid? ProductId { get; set; }
        public required string FileName { get; set; }
        public required string ConfigurationData { get; set; }
        public required string ConfigurationType { get; set; }
        public bool IsChangeFileType { get; set; }
        public string? FileType { get; set; } = string.Empty;

        public static ProductConfigurationDto ToDto(ProductConfiguration input)
        {
            return new ProductConfigurationDto
            {
                Id = input.Id,
                ProductId = input.ProductId,
                FileName = input.FileName,
                ConfigurationData = input.ConfigurationData,
                ConfigurationType = input.ConfigurationType.ToString(),
                IsChangeFileType = input.IsChangeFileType,
                FileType = input.FileType.ToString(),
            };
        }
    }
}
