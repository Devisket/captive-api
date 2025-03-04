namespace Captive.Model.Dto
{
    public class FormCheckDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public required string CheckType { get; set; }
        public required string FormType { get; set; }
        public string? Description { get; set; }
        public required int Quantity { get; set; }
        public required string FileInitial { get; set; }
        public string FormCheckType { get; set; } = string.Empty;
    }
}
