using Captive.Data.Enums;
using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using Captive.Model.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.Orderfiles.Command.CreateCustomOrderFile
{
    public class CreateCustomOrderFileCommandHandler : IRequestHandler<CreateCustomOrderFileCommand, CreateCustomOrderFileCommandResponse>
    {
        private const int MaxFileNameLength = 200;

        private readonly IReadUnitOfWork _readUow;
        private readonly IWriteUnitOfWork _writeUow;

        public CreateCustomOrderFileCommandHandler(IReadUnitOfWork readUow, IWriteUnitOfWork writeUow)
        {
            _readUow = readUow;
            _writeUow = writeUow;
        }

        public async Task<CreateCustomOrderFileCommandResponse> Handle(CreateCustomOrderFileCommand request, CancellationToken cancellationToken)
        {
            var batch = await _readUow.BatchFiles.GetAll()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.BatchId, cancellationToken);

            if (batch == null)
                throw new CaptiveException($"Batch ID: {request.BatchId} doesn't exist.", 404);

            if (batch.BankInfoId != request.BankId)
                throw new CaptiveException("The batch doesn't belong to the selected bank.", 400);

            var product = await _readUow.Products.GetAll()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.ProductId, cancellationToken);

            if (product == null)
                throw new CaptiveException($"Product ID: {request.ProductId} doesn't exist.", 404);

            if (product.BankInfoId != request.BankId)
                throw new CaptiveException("The product doesn't belong to the selected bank.", 400);

            var fileName = BuildFileName(request.FileName, product.ProductName);

            var nameExists = await _readUow.OrderFiles.GetAll()
                .AsNoTracking()
                .AnyAsync(x => x.BatchFileId == batch.Id && x.FileName == fileName, cancellationToken);

            if (nameExists)
                throw new CaptiveException($"An order file named '{fileName}' already exists in this batch.", 400);

            var checkOrders = request.CheckOrders ?? new List<CheckOrderDto>();

            var rowErrors = ValidateRequiredFields(checkOrders);
            if (rowErrors.Any())
                throw new CaptiveException(string.Join(" ", rowErrors), 400);

            var orderFile = new OrderFile
            {
                Id = Guid.NewGuid(),
                BatchFileId = batch.Id,
                ProductId = product.Id,
                FileName = fileName,
                FilePath = string.Empty,
                // Pending = ready for validation (uploaded files reach this state once parsed)
                Status = OrderFilesStatus.Pending,
                IsCustom = true,
                IsValidated = false,
                ProcessDate = DateTime.UtcNow,
            };

            await _writeUow.OrderFiles.AddAsync(orderFile, cancellationToken);

            var floatingCheckOrders = checkOrders.Select(x => ToFloatingCheckOrder(x, orderFile.Id)).ToArray();

            if (floatingCheckOrders.Any())
                await _writeUow.FloatingCheckOrders.AddRange(floatingCheckOrders, cancellationToken);

            // Changes are committed by the DatabasePipeline once the handler returns.
            return new CreateCustomOrderFileCommandResponse
            {
                OrderFileId = orderFile.Id,
                BatchId = batch.Id,
                FileName = orderFile.FileName,
                CheckOrderCount = floatingCheckOrders.Length,
            };
        }

        private static string BuildFileName(string? requestedName, string productName)
        {
            var name = string.IsNullOrWhiteSpace(requestedName)
                ? $"CUSTOM_{productName}_{DateTime.Now:yyyyMMddHHmmss}"
                : requestedName.Trim();

            foreach (var invalidChar in Path.GetInvalidFileNameChars())
                name = name.Replace(invalidChar, '_');

            if (name.Length > MaxFileNameLength)
                name = name.Substring(0, MaxFileNameLength);

            return name;
        }

        private static List<string> ValidateRequiredFields(IList<CheckOrderDto> checkOrders)
        {
            var errors = new List<string>();

            for (int i = 0; i < checkOrders.Count; i++)
            {
                var row = checkOrders[i];
                var missing = new List<string>();

                if (string.IsNullOrWhiteSpace(row.AccountNumber)) missing.Add("account number");
                if (string.IsNullOrWhiteSpace(row.BRSTN)) missing.Add("BRSTN");
                if (string.IsNullOrWhiteSpace(row.CheckType) || string.IsNullOrWhiteSpace(row.FormType)) missing.Add("check/form type");
                if (row.Quantity <= 0) missing.Add("quantity");

                if (missing.Any())
                    errors.Add($"Row {i + 1}: missing {string.Join(", ", missing)}.");
            }

            return errors;
        }

        private static FloatingCheckOrder ToFloatingCheckOrder(CheckOrderDto dto, Guid orderFileId)
        {
            var accountName1 = dto.AccountName1?.Trim() ?? string.Empty;
            var accountName2 = dto.AccountName2?.Trim() ?? string.Empty;

            var mainAccountName = !string.IsNullOrWhiteSpace(dto.MainAccountName)
                ? dto.MainAccountName.Trim()
                : string.Join(" ", new[] { accountName1, accountName2 }.Where(x => !string.IsNullOrEmpty(x)));

            return new FloatingCheckOrder
            {
                Id = Guid.NewGuid(),
                OrderFileId = orderFileId,
                AccountNo = dto.AccountNumber.Trim(),
                BRSTN = dto.BRSTN.Trim(),
                BranchCode = dto.BranchCode?.Trim(),
                AccountName = mainAccountName,
                AccountName1 = accountName1,
                AccountName2 = accountName2,
                CheckType = dto.CheckType.Trim(),
                FormType = dto.FormType.Trim(),
                Quantity = dto.Quantity,
                DeliverTo = string.IsNullOrWhiteSpace(dto.DeliverTo) ? null : dto.DeliverTo.Trim(),
                Concode = string.IsNullOrWhiteSpace(dto.Concode) ? null : dto.Concode.Trim(),
                PreStartingSeries = string.IsNullOrWhiteSpace(dto.StartingSeries) ? null : dto.StartingSeries.Trim(),
                PreEndingSeries = string.IsNullOrWhiteSpace(dto.EndingSeries) ? null : dto.EndingSeries.Trim(),
                OrderNo = string.IsNullOrWhiteSpace(dto.OrderNo) ? null : dto.OrderNo.Trim(),
                IsValid = false,
                IsOnHold = false,
                ErrorMessage = string.Empty,
            };
        }
    }
}
