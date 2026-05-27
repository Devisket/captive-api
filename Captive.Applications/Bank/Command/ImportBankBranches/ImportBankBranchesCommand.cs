using Captive.Model.Dto;
using MediatR;

namespace Captive.Applications.Bank.Command.ImportBankBranches
{
    public class ImportBankBranchesCommand : IRequest<ImportBankBranchResult>
    {
        public Guid BankId { get; set; }
        public Stream FileStream { get; set; } = null!;
    }
}
