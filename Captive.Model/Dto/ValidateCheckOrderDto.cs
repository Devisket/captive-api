
namespace Captive.Model.Dto
{
    public class ValidateCheckOrderDto
    {
        public Guid OrderId { get; set; }
        public bool IsValid { get; set; }
        public CheckOrderDto[] CheckOrder { get; set; }
    }
}
