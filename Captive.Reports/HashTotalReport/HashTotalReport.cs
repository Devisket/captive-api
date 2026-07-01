using Captive.Data.Enums;
using Captive.Data.Models;

namespace Captive.Reports.HashTotalReport
{
    public class HashTotalReport : IHashTotalReport
    {
        private readonly IReportService _reportService;

        public HashTotalReport(IReportService reportService)
        {
            _reportService = reportService;
        }

        public async Task GenerateReport(BatchFile batchFile, ICollection<CheckOrders> checkOrders, string filePath, CancellationToken cancellationToken)
        {
            var checkDto = await _reportService.ExtractCheckOrderDto(checkOrders, batchFile.BankInfoId, cancellationToken);

            var ordered = checkDto
                .OrderBy(x => x.ProductSequence)
                .ThenBy(x => x.BankBranch.BRSTNCode)
                .ThenBy(x => x.CheckOrder.AccountNo)
                .ThenBy(x => x.StartSeries)
                .ToList();

            var outputFile = Path.Combine(filePath, "HashTotal.txt");

            long sumAcctNo = 0;
            var totalCount = 0;

            using var writer = new StreamWriter(outputFile, false);

            writer.WriteLine($"H{DateTime.Now:MMddyyyy}");

            foreach (var record in ordered)
            {
                var acctNo = record.CheckOrder.AccountNo.PadLeft(12, '0');
                var brstn = record.BankBranch.BRSTNCode.PadLeft(9, '0');
                var startSeries = record.StartSeries.PadLeft(10, '0');
                var endSeries = record.EndSeries.PadLeft(10, '0');
                var type = record.FormCheckType == FormCheckType.Personal ? "1" : "2";

                writer.WriteLine($"{acctNo}{brstn}{startSeries}{endSeries}{type}");

                var numericAcct = new string(record.CheckOrder.AccountNo.Where(char.IsDigit).ToArray());
                if (long.TryParse(numericAcct, out var parsed))
                    sumAcctNo += parsed;

                totalCount++;
            }

            writer.WriteLine($"T{sumAcctNo.ToString().PadLeft(15, '0')}{totalCount.ToString().PadLeft(6, '0')}");
        }
    }
}
