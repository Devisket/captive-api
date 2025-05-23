﻿namespace Captive.Model.Request
{
    public class ProductConfigurationRequest
    {     
        public Guid? Id { get; set; }
        public required string FileName {  get; set; } = string.Empty;
        public required string ConfigurationData { get; set; } = string.Empty ;
        public required string ConfigurationType { get; set; }
        public bool IsChangeFileType { get; set; }
        public string FileType { get; set; } = string.Empty;
    }
}
