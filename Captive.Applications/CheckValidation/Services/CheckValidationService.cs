using Captive.Applications.Util;
using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.CheckValidation.Services
{
    
    public interface ICheckValidationService
    {
        Task<bool> HasConflictedSeries(Guid CheckInventoryId, string startingSeries, string endingSeries, Guid branchId, Guid formcheckId, Guid productId, Guid tagId, CancellationToken cancellationToken);
        Tag GetTag(Tag[] tags, Guid branchId, Guid formCheckId, Guid productId);
    }
    public class CheckValidationService : ICheckValidationService
    {
        private readonly IReadUnitOfWork _readUow;
        private readonly IStringService _stringService;

        public CheckValidationService(IReadUnitOfWork readUow, IStringService stringService) 
        { 
            _readUow = readUow;
            _stringService = stringService;
        }

        public async Task<bool>HasConflictedSeries(Guid CheckInventoryId, string startingSeries, string endingSeries, Guid branchId, Guid formcheckId, Guid productId, Guid tagId , CancellationToken cancellationToken)
        {
            var checkInventory = await _readUow.CheckInventory
                .GetAll()
                .Include(x => x.CheckInventoryDetails)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (checkInventory.CheckInventoryDetails == null || !checkInventory.CheckInventoryDetails.Any())
                return false;

            var checkInventoryDetails = checkInventory.CheckInventoryDetails
                .Where(x => x.BranchId == branchId && x.FormCheckId == formcheckId && productId == x.ProductId && x.TagId == tagId)
                .ToList();

            if(!checkInventoryDetails.Any())
                return false;

            var numberSeries = _stringService.ExtractNumber(checkInventory.SeriesPatern, startingSeries, endingSeries);

            return checkInventoryDetails.Any(x =>
            (x.EndingNumber >= numberSeries.Item1 && x.StartingNumber <= numberSeries.Item1) ||
            (x.EndingNumber >= numberSeries.Item2 && x.StartingNumber <= numberSeries.Item2));
        }

        public Tag GetTag(Tag[] tags, Guid branchId, Guid formCheckId, Guid productId)
        {
            if (!tags.Any(x => !x.isDefaultTag))
                return tags.First();

            var flatTagMapping = tags.SelectMany(x => x.Mapping, (parent, child) => new {
                tag = parent,
                child.BranchId,
                child.FormCheckId,
                child.ProductId,
            });

            var searchtag = flatTagMapping.FirstOrDefault(x => x.BranchId == branchId && x.FormCheckId == formCheckId && x.ProductId == productId);

            if (searchtag == null)
                return tags.Where(x => x.isDefaultTag).First();

            return searchtag.tag;
        }
    }
}
