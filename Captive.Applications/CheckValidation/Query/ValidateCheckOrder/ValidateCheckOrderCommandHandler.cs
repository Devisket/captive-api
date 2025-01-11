using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using Captive.Model.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace Captive.Applications.CheckValidation.Query.ValidateCheckOrder
{
    public class ValidateCheckOrderCommandHandler : IRequestHandler<ValidateCheckOrderCommand, ValidateCheckOrderDto>
    {

        private readonly IWriteUnitOfWork _writeUow;
        private readonly IReadUnitOfWork _readUow;

        public ValidateCheckOrderCommandHandler(IWriteUnitOfWork writeUow, IReadUnitOfWork readUow)
        {
            _writeUow = writeUow;
            _readUow = readUow;
        }

        public async Task<ValidateCheckOrderDto> Handle(ValidateCheckOrderCommand request, CancellationToken cancellationToken)
        {
            var orderFile = _readUow.OrderFiles.GetAll()
                .Include(x => x.Product)
                .Include(x => x.CheckOrders)
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == request.OrderId);
            
            if (orderFile == null || orderFile.CheckOrders == null) 
            {
                return new ValidateCheckOrderDto
                {
                    IsValid = false,
                    OrderId = request.OrderId
                };
            }

            var checkOrders = orderFile.CheckOrders.ToList();

            var brstns = await _readUow.BankBranches.GetAll().AsNoTracking().Where(x => x.BankInfoId == orderFile.Product.BankInfoId).Select(x => x.BRSTNCode).ToArrayAsync(cancellationToken);

            var productConfiguration = await _readUow.ProductConfigurations.GetAll().AsNoTracking().FirstOrDefaultAsync(x => x.ProductId == orderFile.ProductId, cancellationToken);

            var productFormChecks = await _readUow.FormChecks.GetAll().AsNoTracking().Where(x => x.ProductId == orderFile.ProductId).ToArrayAsync(cancellationToken);

            var CheckValidation = _readUow.CheckValidations.GetAll().Include(x => x.Tags).Where(x => x.Id == productConfiguration.Id).ToListAsync(cancellationToken);

            //var checkDtos  = ValidateCheckOrder(checkOrders, brstns, productFormChecks);

            //return new ValidateCheckOrderDto
            //{
            //    IsValid = !checkDtos.Any(x => !x.IsValid),
            //    CheckOrder = checkDtos,
            //    OrderId = request.OrderId,
            //};

            return new ValidateCheckOrderDto
            {
                IsValid = false,
                CheckOrder = null,
                OrderId = request.OrderId,
            };
        }

        //private CheckOrderDto[] ValidateCheckOrder(FloatingCheckOrder[] checkOrders, string[] brstn, Captive.Data.Models.FormChecks[] formChecks)
        //{            
        //    foreach (var checkOrder in checkOrders) 
        //    {
        //        if (String.IsNullOrEmpty(checkOrder.BRSTN))
        //        {
        //            checkOrder.IsValid = false;
        //            checkOrder.ErrorMessage = "BRSTN is empty.";

        //            continue;
        //        }

        //        if (String.IsNullOrEmpty(checkOrder.AccountNumber))
        //        {
        //            checkOrder.IsValid = false;
        //            checkOrder.ErrorMessage = "Account number is empty.";
        //            continue;
        //        }

        //        if (checkOrder.Quantity <= 0)
        //        {
        //            checkOrder.IsValid = false;
        //            checkOrder.ErrorMessage = "Quantity is less than 0.";
        //            continue;
        //        }

        //        if (string.IsNullOrEmpty(checkOrder.FormType))
        //        {
        //            checkOrder.IsValid = false;
        //            checkOrder.ErrorMessage = "Form type is empty.";
        //            continue;
        //        }

        //        if (string.IsNullOrEmpty(checkOrder.CheckType))
        //        {
        //            checkOrder.IsValid = false;
        //            checkOrder.ErrorMessage = "Check type is empty.";
        //            continue;
        //        }

        //        //Validate branch's BRSTN
        //        if (!brstn.Contains(checkOrder.BRSTN))
        //        {
        //            checkOrder.IsValid = false;
        //            checkOrder.ErrorMessage = $"BRSTN CODE {checkOrder.BRSTN} doesn't exist";
        //            continue;
        //        }

        //        //Validate FormCheck
        //        if (!formChecks.Any(x => x.FormType == checkOrder.FormType && x.CheckType == checkOrder.CheckType)) 
        //        {
        //            checkOrder.IsValid = false;
        //            checkOrder.ErrorMessage = $"Check Type: {checkOrder.CheckType} and Form type: {checkOrder.FormType} doesn't exist";
        //            continue;
        //        }

        //        //Validate Check Inventory
        //        /*
        //         * Only validate check inventory when the check order has predefine series
        //         * 1. Get the corresponding check validation thru ProductConfiguration
        //         * 2. Check the CheckValidation if it is Product, Mix, Account or Branch
        //         *  2a. If Mix get the TagMapping under check validation
        //         *  2b. Check the corresponding validation if validated by Branch, Product and FormCheck
        //         *  2c. Get the specific tag mapping according to the validation in the check validation table
        //         *  2d. Validate the series on check inventory details with specific tag
        //         */

        //        if(!String.IsNullOrEmpty(checkOrder.StartingSeries) && !string.IsNullOrEmpty(checkOrder.EndingSeries))
        //        {
        //            if (!ValidateCheckSeries(checkOrder))
        //            {
        //                checkOrder.IsValid = false;
        //                checkOrder.ErrorMessage = $"Check series is conflicted for check order series {checkOrder.StartingSeries} - {checkOrder.EndingSeries}";
        //                continue;
        //            }
        //        }

        //        checkOrder.IsValid = true;
        //        checkOrder.ErrorMessage = string.Empty;
        //    }

        //    return checkOrders;
        //}

        public bool ValidateCheckSeries(CheckOrderDto checkOrder)
        {
            return false;
        }
    }
}
