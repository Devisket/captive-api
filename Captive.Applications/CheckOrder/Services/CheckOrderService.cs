﻿
using Captive.Applications.CheckValidation.Services;
using Captive.Applications.FormsChecks.Services;
using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using Captive.Model.Dto;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.CheckOrder.Services
{
    public class CheckOrderService : ICheckOrderService
    {
        private readonly IReadUnitOfWork _readUow;
        private readonly IWriteUnitOfWork _writeUow;
        private readonly IFormsChecksService _formChecksService;
        private readonly ICheckValidationService _checkValidationService;

        public CheckOrderService(IReadUnitOfWork readUow, IWriteUnitOfWork writeUow, IFormsChecksService formCheckService, ICheckValidationService checkValidationService)
        {
            _readUow = readUow;
            _writeUow = writeUow;
            _formChecksService = formCheckService;
            _checkValidationService = checkValidationService;
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
        public async Task<Tuple<FloatingCheckOrder[],int,int>> ValidateCheckOrder(Guid orderFileId, CancellationToken cancellationToken)
        {

            int personalQuantity = 0, commercialQuantity = 0;

            var orderFile = await _readUow.OrderFiles.GetAll()
                .Include(x => x.Product)
                .Include(x => x.BatchFile)
                .Include(x =>x.Product)
                    .ThenInclude(x =>x.ProductConfiguration)
                .Include(x => x.FloatingCheckOrders).FirstOrDefaultAsync(x => x.Id == orderFileId, cancellationToken);

            if (orderFile == null)
                throw new SystemException($"Order file ID {orderFileId} doesn't exist");

            if (orderFile.FloatingCheckOrders == null || !orderFile.FloatingCheckOrders.Any())
                return null;

            var floatingCheckOrders = orderFile.FloatingCheckOrders.ToArray();

            var formChecks = await _readUow.FormChecks.GetAll().AsNoTracking().Where(x => x.ProductId == orderFile.ProductId).ToArrayAsync(cancellationToken);

            var brstns = await _readUow.BankBranches.GetAll().AsNoTracking().Where(x => x.BankInfoId == orderFile.Product.BankInfoId).Select(x => x.BRSTNCode).ToArrayAsync(cancellationToken);

            var checkValidation = await _readUow.CheckValidations
                .GetAll()
                .Include(x => x.Tags)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == orderFile.Product.ProductConfiguration.CheckValidationId);

            foreach (var checkOrder in floatingCheckOrders)
            {

                if (await HasDuplicate(orderFile.BatchFileId, orderFileId, checkOrder.AccountNo, cancellationToken))
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

                var formCheck = formChecks.First(x => x.FormType == checkOrder.FormType && x.CheckType == checkOrder.CheckType);

                personalQuantity = formCheck.FormCheckType == Data.Enums.FormCheckType.Personal ? personalQuantity += 1 : personalQuantity;
                commercialQuantity = formCheck.FormCheckType == Data.Enums.FormCheckType.Commercial ? commercialQuantity+= 1 : commercialQuantity;


                if (!String.IsNullOrEmpty(checkOrder.PreStartingSeries) && !string.IsNullOrEmpty(checkOrder.PreEndingSeries))
                {
                    if (string.IsNullOrEmpty(checkOrder.PreStartingSeries) || string.IsNullOrEmpty(checkOrder.PreEndingSeries)) 
                    {
                        checkOrder.IsValid = false;
                        checkOrder.ErrorMessage = $"One of the series is empty!";
                        continue;
                    }

                    var tags = checkValidation.Tags.ToArray();

                    var branch = _readUow.BankBranches.GetAll().AsNoTracking().Where(x => x.BRSTNCode == checkOrder.BRSTN && x.BankInfoId == orderFile.BatchFile.BankInfoId).First();
                    
                    var tag = _checkValidationService.GetTag(tags, branch.Id, formCheck.Id, orderFile.ProductId);

                    if (await _checkValidationService.HasConflictedSeries(tag.CheckInventoryId, checkOrder.PreStartingSeries, checkOrder.PreEndingSeries, branch.Id, formCheck.Id,orderFile.ProductId, tag.Id, cancellationToken))
                    {
                        checkOrder.IsValid = false;
                        checkOrder.ErrorMessage = $"Has conflicted series number!";
                        continue;
                    }
                }

                checkOrder.IsValid = true;
                checkOrder.ErrorMessage = string.Empty;
            }

            return new Tuple<FloatingCheckOrder[], int, int>(floatingCheckOrders, personalQuantity,commercialQuantity);
        }
        private async Task<bool> HasDuplicate(Guid batchId, Guid orderFileId, string accNo, CancellationToken cancellationToken)
        {
            var otherOrderFile = await _readUow.OrderFiles.GetAll()
                .Include(x => x.FloatingCheckOrders)
                .AsNoTracking()
                .Where(x => x.BatchFileId == batchId && x.Id != orderFileId).ToListAsync(cancellationToken);

            var otherCheckOrder = otherOrderFile.SelectMany(p => p.FloatingCheckOrders, (parent, child) => new { OrderFileId = parent.Id, child.AccountNo });

            return otherCheckOrder.Any(x => x.AccountNo == accNo);
        }
        public async Task CreateCheckOrder(OrderFile orderFile, CancellationToken cancellationToken)
        {
            var branchs = await _readUow.BankBranches.GetAll().AsNoTracking().Where(x => x.BankInfoId == orderFile.BatchFile.BankInfoId).ToListAsync(cancellationToken);

            if (orderFile == null)
            {
                throw new Exception($"Order file ID: {orderFile.Id} doesn't exist");
            }

            var floatingCheckOrders = orderFile.FloatingCheckOrders.ToArray();

            List<Captive.Data.Models.CheckOrders> newCheckOrders = new List<CheckOrders>();


            foreach (var checkOrder in floatingCheckOrders)
            {
                var branch = branchs.First(x => x.BRSTNCode == checkOrder.BRSTN);
                var formCheck = await _formChecksService.GetCheckOrderFormCheck(orderFile.ProductId, checkOrder.FormType, checkOrder.CheckType, cancellationToken);

                newCheckOrders.Add(new CheckOrders
                {
                    Id = Guid.Empty,
                    AccountNo = checkOrder.AccountNo,
                    BranchId = branch?.Id ?? Guid.Empty,
                    Quantity = checkOrder.Quantity,
                    PreEndingSeries = checkOrder.PreEndingSeries,
                    PreStartingSeries = checkOrder.PreEndingSeries,
                    AccountName = string.Concat(checkOrder.AccountName1, checkOrder.AccountName2),
                    BRSTN = checkOrder.BRSTN,
                    OrderQuanity = checkOrder.Quantity,
                    FormCheckId = formCheck?.Id ?? null,
                    DeliverTo = checkOrder.DeliverTo,
                    Concode = checkOrder.Concode,
                    OrderFileId = orderFile.Id,
                    ProductId = orderFile.ProductId,
                    BranchCode = checkOrder.BranchCode ?? string.Empty,
                });
            }

            if (newCheckOrders == null || !newCheckOrders.Any())
                throw new Exception("Cannot create check order");

            await _writeUow.CheckOrders.AddRange(newCheckOrders.ToArray(), cancellationToken);
        }
    }
}
