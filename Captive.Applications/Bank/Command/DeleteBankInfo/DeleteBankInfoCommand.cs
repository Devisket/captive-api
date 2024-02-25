using MediatR;

namespace Captive.Applications.Bank.Command.DeleteBankInfo
{
    public class DeleteBankInfoCommand:IRequest<Unit>
    {
        public DeleteBankInfoCommand (int id)
        {
            this.id = id;
        }

        public int id { get; set; }
    }
}
