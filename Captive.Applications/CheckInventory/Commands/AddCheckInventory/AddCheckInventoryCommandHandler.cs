using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using MediatR;
using Microsoft.EntityFrameworkCore;
using CheckInventoryModel = Captive.Data.Models.CheckInventory;
using CheckInventoryMappingModel = Captive.Data.Models.CheckInventoryMapping;

namespace Captive.Applications.CheckInventory.Commands.AddCheckInventory
{
    public class AddCheckInventoryCommandHandler : IRequestHandler<AddCheckInventoryCommand, Unit>
    {
        private readonly IReadUnitOfWork _readUow;
        private readonly IWriteUnitOfWork _writeUow;

        public AddCheckInventoryCommandHandler(IWriteUnitOfWork writeUow, IReadUnitOfWork readUow)
        {
            _writeUow = writeUow;
            _readUow = readUow;
        }

        public async Task<Unit> Handle(AddCheckInventoryCommand request, CancellationToken cancellationToken)
        {
            if (request.Id.HasValue)
            {
                var checkInventory = await _readUow.CheckInventory.GetAll()
                    .FirstOrDefaultAsync(x => x.Id == request.Id.Value, cancellationToken);

                if (checkInventory == null)
                    throw new Exception($"Check Inventory ID: {request.Id} doesn't exist");

                checkInventory.WarningSeries = request.WarningSeries;
                checkInventory.SeriesPatern = request.SeriesPattern ?? string.Empty;
                checkInventory.StartingSeries = request.StartingSeries;
                checkInventory.EndingSeries = request.EndingSeries;
                checkInventory.NumberOfPadding = request.NumberOfPadding;
                checkInventory.isRepeating = request.IsRepeating;
                checkInventory.AccountNumber = request.AccountNumber;

                _writeUow.CheckInventory.Update(checkInventory);

                // Replace mappings
                var existingMappings = await _writeUow.CheckInventoryMappings.GetAll()
                    .Where(m => m.CheckInventoryId == checkInventory.Id)
                    .ToListAsync(cancellationToken);

                foreach (var m in existingMappings)
                    _writeUow.CheckInventoryMappings.Delete(m);

                await AddMappingsAsync(checkInventory.Id, request, cancellationToken);
            }
            else
            {
                var newId = Guid.NewGuid();
                var newCheckInventory = new CheckInventoryModel
                {
                    Id = newId,
                    BankId = request.BankId,
                    SeriesPatern = request.SeriesPattern ?? string.Empty,
                    WarningSeries = request.WarningSeries,
                    StartingSeries = request.StartingSeries,
                    EndingSeries = request.EndingSeries,
                    CurrentSeries = request.StartingSeries,
                    NumberOfPadding = request.NumberOfPadding,
                    isRepeating = request.IsRepeating,
                    IsActive = true,
                    AccountNumber = request.AccountNumber,
                };

                await _writeUow.CheckInventory.AddAsync(newCheckInventory, cancellationToken);
                await AddMappingsAsync(newId, request, cancellationToken);
            }

            return Unit.Value;
        }

        private async Task AddMappingsAsync(Guid checkInventoryId, AddCheckInventoryCommand request, CancellationToken cancellationToken)
        {
            if (request.MappingData == null) return;

            foreach (var branchId in request.MappingData.BranchIds)
                await _writeUow.CheckInventoryMappings.AddAsync(new CheckInventoryMappingModel { Id = Guid.NewGuid(), CheckInventoryId = checkInventoryId, BranchId = branchId }, cancellationToken);

            foreach (var productId in request.MappingData.ProductIds)
                await _writeUow.CheckInventoryMappings.AddAsync(new CheckInventoryMappingModel { Id = Guid.NewGuid(), CheckInventoryId = checkInventoryId, ProductId = productId }, cancellationToken);

            foreach (var formCheckType in request.MappingData.FormCheckType)
                await _writeUow.CheckInventoryMappings.AddAsync(new CheckInventoryMappingModel { Id = Guid.NewGuid(), CheckInventoryId = checkInventoryId, FormCheckType = formCheckType }, cancellationToken);
        }
    }
}
