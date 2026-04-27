using Captive.Data.Models;

namespace Captive.Model.Dto
{
    public class CheckInventoryDto
    {
        public Guid? Id { get; set; }
        public Guid BankId { get; set; }
        public string? SeriesPattern { get; set; }
        public long WarningSeries { get; set; }
        public int NumberOfPadding { get; set; }
        public long StartingSeries { get; set; }
        public long EndingSeries { get; set; }
        public long CurrentSeries { get; set; }
        public bool IsRepeating { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeprecated { get; set; }
        public string? AccountNumber { get; set; }
        public CheckInventoryMappingData? MappingData { get; set; }

        public static CheckInventoryDto ToDto(CheckInventory input)
        {
            return new CheckInventoryDto
            {
                Id = input.Id,
                BankId = input.BankId,
                WarningSeries = input.WarningSeries,
                NumberOfPadding = input.NumberOfPadding,
                StartingSeries = input.StartingSeries,
                EndingSeries = input.EndingSeries,
                CurrentSeries = input.CurrentSeries,
                IsActive = input.IsActive,
                IsDeprecated = input.IsDeprecated,
                AccountNumber = input.AccountNumber,
                MappingData = new CheckInventoryMappingData(
                    input.Mappings.Where(m => m.BranchId.HasValue).Select(m => m.BranchId!.Value),
                    input.Mappings.Where(m => m.ProductId.HasValue).Select(m => m.ProductId!.Value),
                    input.Mappings.Where(m => m.FormCheckType != null).Select(m => m.FormCheckType!)
                ),
                IsRepeating = input.isRepeating,
                SeriesPattern = input.SeriesPatern
            };
        }
    }
}
