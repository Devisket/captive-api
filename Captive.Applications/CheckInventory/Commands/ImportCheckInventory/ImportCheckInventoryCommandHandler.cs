using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using Captive.Model.Dto;
using CsvHelper;
using CsvHelper.Configuration;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using CheckInventoryModel = Captive.Data.Models.CheckInventory;
using ProductModel = Captive.Data.Models.Product;
using BankBranchesModel = Captive.Data.Models.BankBranches;

namespace Captive.Applications.CheckInventory.Commands.ImportCheckInventory
{
    public class ImportCheckInventoryCommandHandler : IRequestHandler<ImportCheckInventoryCommand, ImportCheckInventoryResult>
    {
        private readonly IReadUnitOfWork _readUow;
        private readonly IWriteUnitOfWork _writeUow;

        public ImportCheckInventoryCommandHandler(IReadUnitOfWork readUow, IWriteUnitOfWork writeUow)
        {
            _readUow = readUow;
            _writeUow = writeUow;
        }

        public async Task<ImportCheckInventoryResult> Handle(ImportCheckInventoryCommand request, CancellationToken cancellationToken)
        {
            var result = new ImportCheckInventoryResult();

            var branches = await _readUow.BankBranches.GetAll()
                .Where(x => x.BankInfoId == request.BankId)
                .ToListAsync(cancellationToken);

            var products = await _readUow.Products.GetAll()
                .Where(x => x.BankInfoId == request.BankId)
                .Include(x => x.ProductConfiguration)
                .ToListAsync(cancellationToken);

            var existingInventories = await _writeUow.CheckInventory.GetAll()
                .Where(x => x.BankId == request.BankId && !x.IsDeprecated)
                .Include(x => x.Mappings)
                .ToListAsync(cancellationToken);

            var newlyAddedIds = new HashSet<Guid>();

            List<CheckInventoryCsvRow> rows;
            try
            {
                rows = ParseCsv(request.FileStream);
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Failed to parse file: {ex.Message}");
                return result;
            }

            if (rows.Count == 0)
            {
                result.Errors.Add("File is empty or contains no valid rows.");
                return result;
            }

            foreach (var (row, index) in rows.Select((r, i) => (r, i + 2)))
            {
                try
                {
                    var errorCountBefore = result.Errors.Count;

                    var resolvedBranchIds = ResolveBranchIds(row.BRSTNCodes, branches, index, result);
                    var resolvedProductIds = ResolveProductIds(row.ProductCodes, products, index, result);
                    var resolvedFormCheckTypes = ResolveFormCheckTypes(row.FormCheckTypes, index, result);

                    if (result.Errors.Count > errorCountBefore)
                        continue;

                    var startingSeries = row.StartingSeries;
                    var endingSeries = row.EndingSeries ?? long.MaxValue;
                    var warningSeries = row.WarningSeries ?? (long)(startingSeries + (endingSeries == long.MaxValue ? 10000 : (endingSeries - startingSeries) * 0.9));

                    var conflicts = FindConflicts(existingInventories, resolvedBranchIds, resolvedProductIds, resolvedFormCheckTypes, row.AccountNumber);
                    foreach (var conflict in conflicts)
                    {
                        conflict.IsDeprecated = true;
                        if (!newlyAddedIds.Contains(conflict.Id))
                            _writeUow.CheckInventory.Update(conflict);
                        result.Deprecated++;
                    }

                    var newId = Guid.NewGuid();
                    var newInventory = new CheckInventoryModel
                    {
                        Id = newId,
                        BankId = request.BankId,
                        SeriesPatern = string.IsNullOrWhiteSpace(row.SeriesPattern) ? string.Empty : row.SeriesPattern.ToUpperInvariant(),
                        NumberOfPadding = row.NumberOfPadding,
                        StartingSeries = startingSeries,
                        EndingSeries = endingSeries,
                        WarningSeries = warningSeries,
                        CurrentSeries = startingSeries,
                        isRepeating = row.IsRepeating,
                        IsActive = true,
                        IsDeprecated = false,
                        AccountNumber = string.IsNullOrWhiteSpace(row.AccountNumber) ? null : row.AccountNumber.Trim(),
                    };

                    await _writeUow.CheckInventory.AddAsync(newInventory, cancellationToken);

                    var mappings = BuildMappings(newId, resolvedBranchIds, resolvedProductIds, resolvedFormCheckTypes);
                    foreach (var mapping in mappings)
                        await _writeUow.CheckInventoryMappings.AddAsync(mapping, cancellationToken);

                    newInventory.Mappings = mappings;
                    newlyAddedIds.Add(newId);
                    existingInventories.Add(newInventory);
                    result.Created++;
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Row {index}: {ex.Message}");
                }
            }

            return result;
        }

        private static List<CheckInventoryMapping> BuildMappings(Guid checkInventoryId, List<Guid> branchIds, List<Guid> productIds, List<string> formCheckTypes)
        {
            var mappings = new List<CheckInventoryMapping>();
            mappings.AddRange(branchIds.Select(id => new CheckInventoryMapping { Id = Guid.NewGuid(), CheckInventoryId = checkInventoryId, BranchId = id }));
            mappings.AddRange(productIds.Select(id => new CheckInventoryMapping { Id = Guid.NewGuid(), CheckInventoryId = checkInventoryId, ProductId = id }));
            mappings.AddRange(formCheckTypes.Select(t => new CheckInventoryMapping { Id = Guid.NewGuid(), CheckInventoryId = checkInventoryId, FormCheckType = t }));
            return mappings;
        }

        private static List<CheckInventoryCsvRow> ParseCsv(Stream stream)
        {
            using var reader = new StreamReader(stream, leaveOpen: true);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                HeaderValidated = null,
                TrimOptions = TrimOptions.Trim,
            };
            using var csv = new CsvReader(reader, config);
            return csv.GetRecords<CheckInventoryCsvRow>().ToList();
        }

        private static List<Guid> ResolveBranchIds(string? branchesCell, List<BankBranchesModel> branches, int rowIndex, ImportCheckInventoryResult result)
        {
            if (string.IsNullOrWhiteSpace(branchesCell))
                return new List<Guid>();

            var ids = new List<Guid>();
            foreach (var raw in branchesCell.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                var branch = branches.FirstOrDefault(b =>
                    b.BRSTNCode != null &&
                    b.BRSTNCode.Trim().Equals(raw, StringComparison.OrdinalIgnoreCase));

                if (branch == null)
                    result.Errors.Add($"Row {rowIndex}: BRSTN code '{raw}' does not exist in the database.");
                else
                    ids.Add(branch.Id);
            }
            return ids;
        }

        private static List<Guid> ResolveProductIds(string? productsCell, List<ProductModel> products, int rowIndex, ImportCheckInventoryResult result)
        {
            if (string.IsNullOrWhiteSpace(productsCell))
                return new List<Guid>();

            var ids = new List<Guid>();
            foreach (var raw in productsCell.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                var product = products.FirstOrDefault(p =>
                    p.ProductConfiguration != null &&
                    p.ProductConfiguration.FileName.Trim().Equals(raw, StringComparison.OrdinalIgnoreCase));

                if (product == null)
                    result.Errors.Add($"Row {rowIndex}: Product code '{raw}' does not match any ProductConfiguration FileName in the database.");
                else
                    ids.Add(product.Id);
            }
            return ids;
        }

        private static List<string> ResolveFormCheckTypes(string? formCheckTypesCell, int rowIndex, ImportCheckInventoryResult result)
        {
            if (string.IsNullOrWhiteSpace(formCheckTypesCell))
                return new List<string>();

            var valid = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Personal", "Commercial" };
            var types = new List<string>();
            foreach (var t in formCheckTypesCell.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                if (!valid.Contains(t))
                    result.Errors.Add($"Row {rowIndex}: FormCheckType '{t}' is not valid (accepted values: Personal, Commercial).");
                else
                    types.Add(t);
            }
            return types;
        }

        private static List<CheckInventoryModel> FindConflicts(
            List<CheckInventoryModel> existing,
            List<Guid> newBranchIds,
            List<Guid> newProductIds,
            List<string> newFormCheckTypes,
            string? newAccountNumber)
        {
            return existing.Where(inv =>
            {
                if (inv.IsDeprecated) return false;

                var existingAccount = string.IsNullOrWhiteSpace(inv.AccountNumber) ? null : inv.AccountNumber.Trim();
                var newAccount = string.IsNullOrWhiteSpace(newAccountNumber) ? null : newAccountNumber.Trim();
                if (existingAccount != newAccount) return false;

                var existingBranchIds = inv.Mappings.Where(m => m.BranchId.HasValue).Select(m => m.BranchId!.Value).ToList();
                var existingProductIds = inv.Mappings.Where(m => m.ProductId.HasValue).Select(m => m.ProductId!.Value).ToList();
                var existingFormCheckTypes = inv.Mappings.Where(m => m.FormCheckType != null).Select(m => m.FormCheckType!).ToList();

                bool branchConflicts = !existingBranchIds.Any() || !newBranchIds.Any()
                    || existingBranchIds.Any(id => newBranchIds.Contains(id));

                bool productConflicts = !existingProductIds.Any() || !newProductIds.Any()
                    || existingProductIds.Any(id => newProductIds.Contains(id));

                bool formCheckConflicts = !existingFormCheckTypes.Any() || !newFormCheckTypes.Any()
                    || existingFormCheckTypes.Any(t => newFormCheckTypes.Contains(t, StringComparer.OrdinalIgnoreCase));

                return branchConflicts && productConflicts && formCheckConflicts;
            }).ToList();
        }
    }
}
