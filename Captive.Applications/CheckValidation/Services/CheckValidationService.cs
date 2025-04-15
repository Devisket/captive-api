using Captive.Applications.Util;
using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using Captive.Model.Dto;
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
        Task<Tag> GetTag(Guid bankId, Guid branchId, Guid formCheckId, Guid productId, CancellationToken cancellationToken);
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

        public async Task<bool> HasConflictedSeries(string startingSeries, string endingSeries, Guid branchId, Guid formcheckId, Guid productId, Guid tagId, CancellationToken cancellationToken)
        {
            var checkInventory = await _readUow.CheckInventory
                .GetAll()
                .Include(x => x.Tag)
                .Include(x => x.CheckInventoryDetails)
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.TagId == tagId 
                    && x.IsActive, cancellationToken
                );

            if (checkInventory == null)
                throw new CaptiveException("Can't find the check inventory");

            var tag = checkInventory.Tag;

            if (checkInventory!.CheckInventoryDetails == null || !checkInventory.CheckInventoryDetails.Any())
                return false;

            // Get both local and database records
            var localQuery = checkInventory.CheckInventoryDetails.AsQueryable();
            var dbQuery = _readUow.CheckInventoryDetails.GetAll();

            // Apply filters to both queries based on tag mapping
            localQuery = ApplyFilters(localQuery, tag, branchId, formcheckId, productId);
            dbQuery = ApplyFilters(dbQuery, tag, branchId, formcheckId, productId);

            // Get records from both sources
            var localDetails = localQuery.ToList();
            var dbDetails = await dbQuery.ToListAsync(cancellationToken);

            // Combine and deduplicate records
            var allDetails = localDetails
                .Concat(dbDetails)
                .DistinctBy(x => x.Id)
                .ToList();

            if (!allDetails.Any())
                return false;

            var numberSeries = _stringService.ExtractNumber(checkInventory.SeriesPatern, startingSeries, endingSeries);

            return allDetails.Any(x =>
                (x.EndingNumber >= numberSeries.Item1 && x.StartingNumber <= numberSeries.Item1) ||
                (x.EndingNumber >= numberSeries.Item2 && x.StartingNumber <= numberSeries.Item2));
        }

        private IQueryable<CheckInventoryDetail> ApplyFilters(IQueryable<CheckInventoryDetail> query, Tag tag, Guid branchId, Guid formcheckId, Guid productId)
        {
            // Branch + Product + FormCheck combination
            if (tag.SearchByBranch && tag.SearchByProduct && tag.SearchByFormCheck)
            {
                return query.Where(x => 
                    x.BranchId == branchId && 
                    x.ProductId == productId && 
                    x.FormCheckId == formcheckId);
            }
            // Branch + Product combination
            else if (tag.SearchByBranch && tag.SearchByProduct)
            {
                return query.Where(x => 
                    x.BranchId == branchId && 
                    x.ProductId == productId);
            }
            // Branch + FormCheck combination
            else if (tag.SearchByBranch && tag.SearchByFormCheck)
            {
                return query.Where(x => 
                    x.BranchId == branchId && 
                    x.FormCheckId == formcheckId);
            }
            // Product + FormCheck combination
            else if (tag.SearchByProduct && tag.SearchByFormCheck)
            {
                return query.Where(x => 
                    x.ProductId == productId && 
                    x.FormCheckId == formcheckId);
            }
            // Single criteria
            else
            {
                if (tag.SearchByBranch)
                    query = query.Where(x => x.BranchId == branchId);

                if (tag.SearchByProduct)
                    query = query.Where(x => x.ProductId == productId);

                if (tag.SearchByFormCheck)
                    query = query.Where(x => x.FormCheckId == formcheckId);
            }

            return query;
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

        public async Task<Tag> GetTag(Guid bankId, Guid branchId, Guid formCheckId, Guid productId, CancellationToken cancellationToken)
        {
            var tags = await _readUow.Tags.GetAll().Include(x => x.Mapping).Where(x => x.BankId == bankId).ToListAsync(cancellationToken);

            if (tags == null || !tags.Any())
                return null;

            // Get all non-default tags
            var nonDefaultTags = tags.Where(x => !x.isDefaultTag).ToList();
            if (!nonDefaultTags.Any())
                return tags.First();

            var flatTagMapping = nonDefaultTags.SelectMany(x => x.Mapping, (parent, child) => new {
                tag = parent,
                child.BranchId,
                child.FormCheckId,
                child.ProductId,
            });

            // Priority 1: Exact match with all three IDs
            var exactMatch = flatTagMapping.FirstOrDefault(x => 
                x.BranchId == branchId && 
                x.FormCheckId == formCheckId && 
                x.ProductId == productId);
            if (exactMatch != null)
                return exactMatch.tag;

            // Priority 2: Matches with two IDs
            var twoIdMatches = flatTagMapping.Where(x => 
                (x.BranchId == branchId && x.FormCheckId == formCheckId && x.ProductId == null) ||
                (x.BranchId == branchId && x.ProductId == productId && x.FormCheckId == null) ||
                (x.FormCheckId == formCheckId && x.ProductId == productId && x.BranchId == null))
                .ToList();

            if (twoIdMatches.Any())
                return twoIdMatches.First().tag;

            // Priority 3: Matches with single ID (other IDs must be null)
            var singleIdMatches = flatTagMapping.Where(x => 
                (x.BranchId == branchId && x.FormCheckId == null && x.ProductId == null) ||
                (x.FormCheckId == formCheckId && x.BranchId == null && x.ProductId == null) ||
                (x.ProductId == productId && x.BranchId == null && x.FormCheckId == null))
                .ToList();

            if (singleIdMatches.Any())
                return singleIdMatches.First().tag;

            // Priority 4: Default tag
            return tags.Where(x => x.isDefaultTag).First();
        }
    }
}
