
namespace Captive.Applications.FormChecks.Command.CreateUpdateFormCheck
{
    public class CreateUpdateFormCheckCommandRequest
    {
        public Guid? Id { get; set; }
        public required string CheckType { get; set; }
        public required string FormType { get; set; }
        public required string Description { get; set; }
        public int Quantity { get; set; }
        public required string FormCheckType { get; set; }
        public string? FileInitial { get; set; }
    }
}
