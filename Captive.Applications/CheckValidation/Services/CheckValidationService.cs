using Captive.Applications.Util;
using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace Captive.Applications.CheckValidation.Services
{
    
    public interface ICheckValidationService
    {
        Task<bool> HasConflictedSeries(
            string startingSeries, 
            string endingSeries, 
            Guid branchId, 
            Guid formcheckId, 
            Guid productId, 
            Guid tagId, 
            CancellationToken cancellationToken);
        string HitWarningSeries(Captive.Data.Models.CheckInventory checkInventory, string startingSeries, string endingSeries);
        bool HitEndingSeries(Captive.Data.Models.CheckInventory checkInventory, string startingSeries, string endingSeries);
        Tag? GetTag(Guid bankId, Guid branchId, Guid formCheckId, Guid productId);
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

        public async Task<bool>HasConflictedSeries(string startingSeries, string endingSeries, Guid branchId, Guid formcheckId, Guid productId, Guid tagId , CancellationToken cancellationToken)
        {
            var checkInventory = await _readUow.CheckInventory
                .GetAll()
                .Include(x => x.Tag)
                .Include(x => x.CheckInventoryDetails)
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.TagId == tagId 
                    && x.IsEnable, cancellationToken
                );

            if (checkInventory!.CheckInventoryDetails == null || !checkInventory.CheckInventoryDetails.Any())
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

        /*
         * Create function to tell whenever we hit the warning (Throw as warning)
         * 
         * Calls: 
         * - Validating Check Order
         * - Processing Check Order
         */
        public string HitWarningSeries(Captive.Data.Models.CheckInventory checkInventory, string startingSeries, string endingSeries)
        {
            var numberSeries = _stringService.ExtractNumber(checkInventory.SeriesPatern, startingSeries, endingSeries);

            if (numberSeries.Item2 >= checkInventory.WarningSeries)
                return "Check inventory almost low";

            return string.Empty;
        }

        /*
         * Create a function to tell whenever we are out of check inventory (Throw as erro)
         * 
         *Calls: 
         * - Validating Check Order
         * - Processing Check Order
         */
        public bool HitEndingSeries(Captive.Data.Models.CheckInventory checkInventory, string startingSeries, string endingSeries)
        {
            var numberSeries = _stringService.ExtractNumber(checkInventory.SeriesPatern, startingSeries, endingSeries);

            if (numberSeries.Item2 >= checkInventory.EndingSeries)
                return true;

            return false;
        }

        public Tag? GetTag(Guid bankId, Guid branchId, Guid formCheckId, Guid productId)
        {
            var tags = _readUow.Tags.GetAll().Include(x => x.Mapping).Where(x => x.BankId == bankId).ToList();

            if (tags == null || !tags.Any())
                return null;

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
