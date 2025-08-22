using Captive.Data.Enums;
using Captive.Data.Models;

namespace Captive.Model.Dto.Reports
{
    public class CheckOrderReport
    {
        public required string ProductTypeName { get; set; }
        public string? CustomizeFileName { get; set; }
        public string? FormCheckName { get; set; }
        public string CheckType { get; set; }
        public string FormType { get; set; }
        public required CheckOrders CheckOrder { get; set; }
        public Guid CheckInventoryId { get; set; }
        public string StartSeries { get; set; }
        public long StartNumber { get; set; }
        public string EndSeries { get; set; }
        public string SeriesPattern {  get; set; }
        
        public int NoOfPadding {  get; set; }

        public Guid? OrderFileId { get; set; }
        public string? AccountNumberFormat { get; set; }
        public string? BarcodeValue {  get; set; }
        public FormCheckType FormCheckType { get; set; }
        public string? OrderFileName { get; set; }
        public required string FileInitial { get; set; }
        public required BankBranches BankBranch { get; set; }
        public BankBranches? DeliverTo { get; set; }
    }
}
