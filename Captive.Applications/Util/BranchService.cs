
using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.Util
{
    public interface IBranchService
    {
        Task<string> GetBranchName(Guid bankId, string BRSTN, CancellationToken cancellationToken);
        string GetBranchName(BankBranches[] bankBranches, string BRSTN);
    }
    public class BranchService : IBranchService
    {
        private readonly IReadUnitOfWork _readUow;

        public BranchService(IReadUnitOfWork readUow)
        {
            _readUow = readUow;
        }

        public async Task<string> GetBranchName(Guid bankId, string BRSTN, CancellationToken cancellationToken)
        {
            var branch = await _readUow.BankBranches.GetAll().AsNoTracking().FirstOrDefaultAsync(x => x.BRSTNCode == BRSTN && x.BankInfoId == bankId, cancellationToken);

            if (branch == null)
                throw new Exception($"BRSTN: {BRSTN} doesn't exist");

            return branch.BranchName;
        }

        public string GetBranchName(BankBranches[] bankBranches, string BRSTN)
        {
            var branch = bankBranches.FirstOrDefault(x => x.BRSTNCode == BRSTN);

            if (branch == null)
                throw new Exception($"BRSTN: {BRSTN} doesn't exist");

            return branch.BranchName;
        }
    }
}
