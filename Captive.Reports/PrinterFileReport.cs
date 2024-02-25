using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using Captive.Processing.Processor.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;

namespace Captive.Reports
{
    public class PrinterFileReport : IPrinterFileReport
    {
        private readonly IWriteUnitOfWork _writeUow;
        private readonly IReadUnitOfWork _readUow;
        private readonly IConfiguration _configuration;

        public PrinterFileReport(
            IWriteUnitOfWork writeUow,
            IReadUnitOfWork readUow,
            IConfiguration config) 
        {
            _writeUow = writeUow;
            _readUow = readUow;
            _configuration = config;
        }

        public async Task GenerateReport(OrderFile orderFile, BankInfo bank, CancellationToken cancellationToken)
        {
            var checkOrders = _readUow.CheckOrders.GetAll()
                .Include(x => x.FormChecks)
                .AsNoTracking()
                .Where(x => x.OrderFileId == orderFile.Id);

            var checkInventories = _readUow.CheckInventory.GetAll().Where(x => x.CheckOrderId.HasValue && checkOrders.Any(z => z.Id == x.CheckOrderId.Value));

            var orderSummary = await checkInventories.Join(checkOrders, x => x.CheckOrderId, z => z.Id, (checkInventory, checkOrder) => new
            {
                CheckOrder = checkOrders.First(),
                startingSeries = checkInventory.StarSeries,
                endingSeries = checkInventory.EndSeries
            }).ToListAsync();

            orderSummary.OrderBy(x => x.CheckOrder.FormCheckId).ThenBy(x => x.endingSeries);

            var filePath = _configuration["OutputDirectory"];

            filePath = filePath.Replace("Bank", bank.BankName);
            filePath = filePath.Replace("Date", DateTime.UtcNow.ToString("MM-dd-yyyy"));
            filePath = Path.Combine(filePath, "PrinterFile");


            if(!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            
            using (var streamWriter = new StreamWriter(Path.Combine(filePath, orderFile.FileName)))
            {
                foreach(var order in orderSummary)
                {
                    RenderText(streamWriter, order.CheckOrder, order.startingSeries, order.endingSeries);

                }
            }
        }

        private void RenderText(StreamWriter writer, CheckOrders checkOrder, string? startingSeries, string? endingSeries)
        {
            writer.WriteLine(3);
            writer.WriteLine(checkOrder.BRSTN);
            writer.WriteLine(checkOrder.AccountNo);
            writer.WriteLine(endingSeries);
            writer.WriteLine(checkOrder.FormChecks.CheckType);
            writer.WriteLine();
            writer.WriteLine(checkOrder.BRSTN.Substring(0, 5));
            writer.WriteLine(string.Format(" {0}", checkOrder.BRSTN.Substring(4, 4)));

            var formattedAccNo = Regex.Replace(checkOrder.AccountNo, @"(\w{3})(\w{6})(\w{3})", @"$1-$2-$3");
            writer.WriteLine(formattedAccNo);
            writer.WriteLine(checkOrder.AccountName);
            writer.WriteLine("\n \n \n");

            //Continue

        }

    }
}
