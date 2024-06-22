
using MediatR;

namespace Captive.Applications.Bank.Command.DeleteBankBranch
{
    public  class DeleteBankBranchCommand :IRequest<Unit>
    {
        public Guid BankId { get; set; }
        public Guid BranchId { get; set; }   
    }
}
