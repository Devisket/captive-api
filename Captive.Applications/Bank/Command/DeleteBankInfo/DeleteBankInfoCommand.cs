using MediatR;

namespace Captive.Applications.Bank.Command.DeleteBankInfo
{
    public class DeleteBankInfoCommand:IRequest<Unit>
    {
        public DeleteBankInfoCommand (Guid Id)
        {
            this.Id = Id;
        }

        public Guid Id { get; set; }
    }
}
