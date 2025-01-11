namespace Captive.Applications.CheckValidation.Query.ValidateCheckOrder
{
    public class ValidateCheckOrderResponse
    {
        public bool IsValid {  get; set; }  
        public List<string> ErrorMessages { get; set; }
    }
}
