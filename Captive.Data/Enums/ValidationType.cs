namespace Captive.Data.Enums
{
    public enum ValidationType
    {
        Branch, //Validate check inventory based from branch
        Product, //Validate check inventory based from product
        Account, //Validate check inventory based form account
        FormCheck, //Validate check inventory based from form check 
        Mix // It could be mix of branch, product and form check
    }
}
