using Captive.Applications.Util;
using Captive.Data.Enums;
using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using Captive.Model.Dto;
using Microsoft.EntityFrameworkCore;

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
            Guid checkInventoryId,
            CancellationToken cancellationToken);
        string HitWarningSeries(Captive.Data.Models.CheckInventory checkInventory, string startingSeries, string endingSeries);
        bool HitEndingSeries(Captive.Data.Models.CheckInventory checkInventory, string startingSeries, string endingSeries);
        Task<Captive.Data.Models.CheckInventory> GetCheckInventoryDirect(Guid bankId, Guid branchId, Guid productId, FormCheckType checkType, string? accountNumber, CancellationToken cancellationToken);
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

        public async Task<bool> HasConflictedSeries(string startingSeries, string endingSeries, Guid branchId, Guid formcheckId, Guid productId, Guid checkInventoryId, CancellationToken cancellationToken)
        {
            var checkInventory = await _readUow.CheckInventory
                .GetAll()
                .Include(x => x.CheckInventoryDetails)
                .Include(x => x.Mappings)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == checkInventoryId, cancellationToken);

            if (checkInventory == null)
                throw new CaptiveException("Can't find the check inventory");

            if (checkInventory.CheckInventoryDetails == null || !checkInventory.CheckInventoryDetails.Any())
                return false;

            var mapping = GetMappingData(checkInventory);

            var localQuery = checkInventory.CheckInventoryDetails.AsQueryable();
            var dbQuery = _readUow.CheckInventoryDetails.GetAll();

            localQuery = ApplyFilters(localQuery, mapping, branchId, formcheckId, productId);
            dbQuery = ApplyFilters(dbQuery, mapping, branchId, formcheckId, productId);

            var localDetails = localQuery.ToList();
            var dbDetails = await dbQuery.ToListAsync(cancellationToken);

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

        private IQueryable<CheckInventoryDetail> ApplyFilters(IQueryable<CheckInventoryDetail> query, CheckInventoryMappingData mapping, Guid branchId, Guid formcheckId, Guid productId)
        {
            if (mapping.BranchIds.Any())
                query = query.Where(x => x.BranchId == branchId);

            if (mapping.ProductIds.Any())
                query = query.Where(x => x.ProductId == productId);

            if (mapping.FormCheckType.Any())
                query = query.Where(x => x.FormCheckId == formcheckId);

            return query;
        }

        public string HitWarningSeries(Captive.Data.Models.CheckInventory checkInventory, string startingSeries, string endingSeries)
        {
            var numberSeries = _stringService.ExtractNumber(checkInventory.SeriesPatern, startingSeries, endingSeries);

            if (numberSeries.Item2 >= checkInventory.WarningSeries)
                return "Check inventory almost low";

            return string.Empty;
        }

        public bool HitEndingSeries(Captive.Data.Models.CheckInventory checkInventory, string startingSeries, string endingSeries)
        {
            var numberSeries = _stringService.ExtractNumber(checkInventory.SeriesPatern, startingSeries, endingSeries);
            return numberSeries.Item2 >= checkInventory.EndingSeries;
        }

        public async Task<Captive.Data.Models.CheckInventory> GetCheckInventoryDirect(Guid bankId, Guid branchId, Guid productId, FormCheckType checkType, string? accountNumber, CancellationToken cancellationToken)
        {
            var checkInventories = await _readUow.CheckInventory
                .GetAll()
                .Include(x => x.Mappings)
                .Where(x => x.BankId == bankId && x.IsActive && !x.IsDeprecated)
                .ToListAsync(cancellationToken);

            if (!string.IsNullOrEmpty(accountNumber))
            {
                var accountMatch = checkInventories.FirstOrDefault(x => x.AccountNumber == accountNumber);
                if (accountMatch != null)
                    return accountMatch;
            }

            var scored = checkInventories
                .Where(x => string.IsNullOrEmpty(x.AccountNumber))
                .Select(x => new { Inventory = x, Mapping = GetMappingData(x) })
                .Where(c =>
                    (!c.Mapping.BranchIds.Any() || c.Mapping.BranchIds.Contains(branchId)) &&
                    (!c.Mapping.ProductIds.Any() || c.Mapping.ProductIds.Contains(productId)) &&
                    (!c.Mapping.FormCheckType.Any() || c.Mapping.FormCheckType.Contains(checkType.ToString())))
                .Select(c => new
                {
                    c.Inventory,
                    Score = (c.Mapping.BranchIds.Any() ? 4 : 0) +
                            (c.Mapping.ProductIds.Any() ? 2 : 0) +
                            (c.Mapping.FormCheckType.Any() ? 1 : 0)
                })
                .OrderByDescending(c => c.Score)
                .FirstOrDefault();

            if (scored == null)
                throw new CaptiveException("Can't find the check inventory");

            return scored.Inventory;
        }

        private static CheckInventoryMappingData GetMappingData(Captive.Data.Models.CheckInventory checkInventory)
        {
            return new CheckInventoryMappingData(
                checkInventory.Mappings.Where(m => m.BranchId.HasValue).Select(m => m.BranchId!.Value),
                checkInventory.Mappings.Where(m => m.ProductId.HasValue).Select(m => m.ProductId!.Value),
                checkInventory.Mappings.Where(m => m.FormCheckType != null).Select(m => m.FormCheckType!)
            );
        }
    }
}
