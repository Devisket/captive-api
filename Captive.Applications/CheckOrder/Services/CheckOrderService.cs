
using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using Captive.Model.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using System.Threading;

namespace Captive.Applications.CheckOrder.Services
{
    public class CheckOrderService : ICheckOrderService
    {
        private readonly IReadUnitOfWork _readUow;
        private readonly IWriteUnitOfWork _writeUow;

        public CheckOrderService(IReadUnitOfWork readUow, IWriteUnitOfWork writeUow) 
        {
            _readUow = readUow;
            _writeUow = writeUow;
        }

        public void UpdateCheckOrder(CheckOrderDto checkOrderDto)
        {
            _writeUow.CheckOrders.Update(
                new CheckOrders
                {
                    Id = Guid.Empty,
                    AccountNo = checkOrderDto.AccountNumber,
                    AccountName = string.Concat(checkOrderDto.AccountName1, checkOrderDto.AccountName2),
                    BRSTN = checkOrderDto.BRSTN,
                    OrderQuanity = checkOrderDto.Quantity,
                    FormCheckId = null,
                    DeliverTo = checkOrderDto.DeliverTo,
                    Concode = checkOrderDto.Concode,
                }
              ); 
        }

        public async Task<FloatingCheckOrder[]> ValidateCheckOrder(Guid orderFileId, CancellationToken cancellationToken)
        {
            var orderFile = await _readUow.OrderFiles.GetAll().Include(x=> x.Product).Include(x => x.FloatingCheckOrders).FirstOrDefaultAsync(x => x.Id == orderFileId, cancellationToken);

            if (orderFile == null)
                throw new SystemException($"Order file ID {orderFileId} doesn't exist");

            if (orderFile.FloatingCheckOrders == null || !orderFile.FloatingCheckOrders.Any())
                return null;

            var floatingCheckOrders = orderFile.FloatingCheckOrders.ToArray();
                      
            var formChecks = await _readUow.FormChecks.GetAll().AsNoTracking().Where(x => x.ProductId == orderFile.ProductId).ToArrayAsync(cancellationToken);

            var brstns = await _readUow.BankBranches.GetAll().AsNoTracking().Where(x => x.BankInfoId == orderFile.Product.BankInfoId).Select(x => x.BRSTNCode).ToArrayAsync(cancellationToken);

            //var productConfiguration = await _readUow.ProductConfigurations.GetAll().AsNoTracking().FirstOrDefaultAsync(x => x.ProductId == orderFile.ProductId, cancellationToken);

            //var productFormChecks = await _readUow.FormChecks.GetAll().AsNoTracking().Where(x => x.ProductId == orderFile.ProductId).ToArrayAsync(cancellationToken);

            foreach (var checkOrder in floatingCheckOrders)
            {

                if(await HasDuplicate(orderFile.BatchFileId, orderFileId, checkOrder.AccountNo, cancellationToken))
                {
                    checkOrder.IsValid = false;
                    checkOrder.ErrorMessage = $"It has duplicated account from other order file.";

                    continue;
                }

                if (String.IsNullOrEmpty(checkOrder.BRSTN))
                {
                    checkOrder.IsValid = false;
                    checkOrder.ErrorMessage = "BRSTN is empty.";

                    continue;
                }

                if (String.IsNullOrEmpty(checkOrder.AccountNo))
                {
                    checkOrder.IsValid = false;
                    checkOrder.ErrorMessage = "Account number is empty.";
                    continue;
                }

                if (checkOrder.Quantity <= 0)
                {
                    checkOrder.IsValid = false;
                    checkOrder.ErrorMessage = "Quantity is less than 0.";
                    continue;
                }

                if (string.IsNullOrEmpty(checkOrder.FormType))
                {
                    checkOrder.IsValid = false;
                    checkOrder.ErrorMessage = "Form type is empty.";
                    continue;
                }

                if (string.IsNullOrEmpty(checkOrder.CheckType))
                {
                    checkOrder.IsValid = false;
                    checkOrder.ErrorMessage = "Check type is empty.";
                    continue;
                }

                //Validate branch's BRSTN
                if (!brstns.Contains(checkOrder.BRSTN))
                {
                    checkOrder.IsValid = false;
                    checkOrder.ErrorMessage = $"BRSTN CODE {checkOrder.BRSTN} doesn't exist";
                    continue;
                }

                //Validate FormCheck
                if (!formChecks.Any(x => x.FormType == checkOrder.FormType && x.CheckType == checkOrder.CheckType))
                {
                    checkOrder.IsValid = false;
                    checkOrder.ErrorMessage = $"Check Type: {checkOrder.CheckType} and Form type: {checkOrder.FormType} doesn't exist";
                    continue;
                }

                //Validate Check Inventory
                /*
                 * Only validate check inventory when the check order has predefine series
                 * 1. Get the corresponding check validation thru ProductConfiguration
                 * 2. Check the CheckValidation if it is Product, Mix, Account or Branch
                 *  2a. If Mix get the TagMapping under check validation
                 *  2b. Check the corresponding validation if validated by Branch, Product and FormCheck
                 *  2c. Get the specific tag mapping according to the validation in the check validation table
                 *  2d. Validate the series on check inventory details with specific tag
                 */

                if (!String.IsNullOrEmpty(checkOrder.PreStartingSeries) && !string.IsNullOrEmpty(checkOrder.PreEndingSeries))
                {
                    //if (!ValidateCheckSeries(checkOrder))
                    //{
                    //    checkOrder.IsValid = false;
                    //    checkOrder.ErrorMessage = $"Check series is conflicted for check order series {checkOrder.StartingSeries} - {checkOrder.EndingSeries}";
                    //    continue;
                    //}
                }

                checkOrder.IsValid = true;
                checkOrder.ErrorMessage = string.Empty;
            }

            return floatingCheckOrders;
        }
        
        private async Task<bool> HasDuplicate(Guid batchId, Guid orderFileId, string accNo, CancellationToken cancellationToken)
        {
            var otherOrderFile = await _readUow.OrderFiles.GetAll()
                .Include(x => x.FloatingCheckOrders)
                .AsNoTracking()
                .Where(x => x.BatchFileId == batchId && x.Id != batchId).ToListAsync(cancellationToken);

            var otherCheckOrder = otherOrderFile.SelectMany(p => p.FloatingCheckOrders, (parent, child) => new { OrderFileId = parent.Id, child.AccountNo });

            return otherCheckOrder.Any(x => x.AccountNo == accNo);
        }


    }
}
