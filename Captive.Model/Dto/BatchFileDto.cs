using Captive.Data.Enums;
using Captive.Data.Models;

namespace Captive.Model.Dto
{
    public class BatchFileDto
    {
        public Guid Id { get; set; }
        public required int OrderNumber { get; set; }
        public required string BatchName { get; set; }
        public required DateTime CreatedDate { get; set; }
        public required BatchFileStatus BatchFileStatus { get; set; }
        public ICollection<OrderfileDto>? OrderFiles { get; set; }
    }
}
