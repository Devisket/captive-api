using Captive.Data.Enums;

namespace Captive.Model.Dto
{
    public class BatchFilesDto
    {
        public Guid Id { get; set; }
        public required int OrderNumber { get; set; }
        public required string BatchName { get; set; }
        public required string CreatedDate { get; set; }
        public required string BatchFileStatus { get; set; }
        public required int NoOfFiles { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
