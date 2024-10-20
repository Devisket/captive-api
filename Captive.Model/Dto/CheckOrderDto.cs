namespace Captive.Model.Dto
{
    public class CheckOrderDto
    {
        public int RowNo { get; set; }
        public required string CheckType {  get; set; }
        public required string FormType {  get; set; }
        public required string AccountNumber {  get; set; }
        public required string BRSTN { get; set; }
        public int Quantity {  get; set; }
        public string? DeliverTo { get; set; }
        public string? FileBatchName { get; set; }
        public string? Concode {  get; set; }
        public string? StartingSeries {  get; set; }
        public string? EndingSeries { get; set; }        
        public string? MainAccountName {  get; set; }
        public string? AccountName1 { get; set; }
        public string? AccountName2 { get; set; }

        public bool IsValid {  get; set; }
        public string? ErrorMessage { get; set; }
    }
}
