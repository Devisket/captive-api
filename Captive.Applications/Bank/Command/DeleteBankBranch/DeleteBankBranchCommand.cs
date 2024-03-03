
using MediatR;

namespace Captive.Applications.Bank.Command.DeleteBankBranch
{
    public  class DeleteBankBranchCommand :IRequest<Unit>
    {
        public int BankId { get; set; }
        public int BranchId { get; set; }   
    }
}
