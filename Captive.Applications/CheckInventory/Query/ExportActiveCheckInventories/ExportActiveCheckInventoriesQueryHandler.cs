using Captive.Applications.CheckInventory.Commands.ImportCheckInventory;
using Captive.Data.UnitOfWork.Read;
using CsvHelper;
using CsvHelper.Configuration;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;

namespace Captive.Applications.CheckInventory.Query.ExportActiveCheckInventories
{
    public class ExportActiveCheckInventoriesQueryHandler : IRequestHandler<ExportActiveCheckInventoriesQuery, byte[]>
    {
        private readonly IReadUnitOfWork _readUow;

        public ExportActiveCheckInventoriesQueryHandler(IReadUnitOfWork readUow)
        {
            _readUow = readUow;
        }

        public async Task<byte[]> Handle(ExportActiveCheckInventoriesQuery request, CancellationToken cancellationToken)
        {
            var inventories = await _readUow.CheckInventory.GetAll()
                .AsNoTracking()
                .Include(x => x.Mappings)
                .Where(x => x.BankId == request.BankId && x.IsActive && !x.IsDeprecated)
                .ToListAsync(cancellationToken);

            var branches = await _readUow.BankBranches.GetAll()
                .AsNoTracking()
                .Where(x => x.BankInfoId == request.BankId)
                .ToListAsync(cancellationToken);

            var products = await _readUow.Products.GetAll()
                .AsNoTracking()
                .Include(x => x.ProductConfiguration)
                .Where(x => x.BankInfoId == request.BankId)
                .ToListAsync(cancellationToken);

            var branchMap = branches.ToDictionary(b => b.Id, b => b.BRSTNCode);
            var productMap = products
                .Where(p => p.ProductConfiguration != null)
                .ToDictionary(p => p.Id, p => p.ProductConfiguration.FileName);

            var rows = inventories.Select(inv =>
            {
                var mappingBranchIds = inv.Mappings.Where(m => m.BranchId.HasValue).Select(m => m.BranchId!.Value).ToList();
                var mappingProductIds = inv.Mappings.Where(m => m.ProductId.HasValue).Select(m => m.ProductId!.Value).ToList();
                var formCheckTypes = inv.Mappings.Where(m => m.FormCheckType != null).Select(m => m.FormCheckType!).ToList();

                var brstnCodes = string.Join(";", mappingBranchIds
                    .Where(id => branchMap.ContainsKey(id))
                    .Select(id => branchMap[id]));

                var productCodes = string.Join(";", mappingProductIds
                    .Where(id => productMap.ContainsKey(id))
                    .Select(id => productMap[id]));

                return new CheckInventoryCsvRow
                {
                    SeriesPattern = inv.SeriesPatern,
                    NumberOfPadding = inv.NumberOfPadding,
                    StartingSeries = inv.StartingSeries,
                    EndingSeries = inv.EndingSeries == long.MaxValue ? null : inv.EndingSeries,
                    WarningSeries = inv.WarningSeries,
                    IsRepeating = inv.isRepeating,
                    BRSTNCodes = string.IsNullOrEmpty(brstnCodes) ? null : brstnCodes,
                    ProductCodes = string.IsNullOrEmpty(productCodes) ? null : productCodes,
                    FormCheckTypes = formCheckTypes.Count > 0 ? string.Join(";", formCheckTypes) : null,
                    AccountNumber = inv.AccountNumber,
                };
            }).ToList();

            using var ms = new MemoryStream();
            using var writer = new StreamWriter(ms, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
            var config = new CsvConfiguration(CultureInfo.InvariantCulture);
            using var csv = new CsvWriter(writer, config);
            csv.WriteRecords(rows);
            writer.Flush();
            return ms.ToArray();
        }
    }
}
