namespace Captive.Model.Dto
{
    /// <summary>A series range assigned to a check order (one booklet).</summary>
    public class CheckInventoryDetailsDto
    {
        public Guid Id { get; set; }
        public Guid? CheckInventoryId { get; set; }
        public Guid? CheckOrderId { get; set; }
        public string? StartingSeries { get; set; }
        public string? EndingSeries { get; set; }
        public int Quantity { get; set; }
        public string? AccountNumber { get; set; }
        public string? CheckOrderName { get; set; }
        public string? OrderFileName { get; set; }
        public string? BatchName { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }
}
