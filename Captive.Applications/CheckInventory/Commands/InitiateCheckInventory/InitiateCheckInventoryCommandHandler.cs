using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using Captive.Model.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Captive.Applications.CheckInventory.Commands.InitiateCheckInventory
{
    public class InitiateCheckInventoryCommandHandler : IRequestHandler<InitiateCheckInventoryCommand, Unit>
    {
        private readonly IReadUnitOfWork _readUow;
        private readonly IWriteUnitOfWork _writeUow;

        public InitiateCheckInventoryCommandHandler(IReadUnitOfWork readUow, IWriteUnitOfWork writeUow) 
        { 
            _readUow = readUow;
            _writeUow = writeUow;
        }

        public async Task<Unit> Handle(InitiateCheckInventoryCommand request, CancellationToken cancellationToken)
        {
            await InitiateCheckInventory(request, cancellationToken);

            return Unit.Value;
        }

        private async Task InitiateCheckInventory(InitiateCheckInventoryCommand request, CancellationToken cancellationToken)
        {
            var tag = await _readUow.Tags.GetAll()
                    .Include(x => x.Mapping)
                    .Include(x => x.CheckInventories )
                .FirstOrDefaultAsync(x => x.Id == request.TagId, cancellationToken);

            if (tag == null)
            {
                throw new CaptiveException($"TagID {request.TagId} doesn't exist.");
            }

            if (!tag.IsLock)
            {
                throw new CaptiveException($"Tag must be lock before initiating check inventory");
            }

            if (tag.CheckInventories != null && tag.CheckInventories!.Any())
            {
                throw new CaptiveException($"You can only initiate check inventory once.");
            }

            if (tag.SearchByAccount)
            {
                var newCheckInventory = new Captive.Data.Models.CheckInventory
                {
                    Id = Guid.NewGuid(),
                    CurrentSeries = request.StartingSeries,
                    StartingSeries = request.StartingSeries,
                    EndingSeries = request.EndingSeries,
                    WarningSeries = request.WarningSeries,
                    JsonMappingData = string.Empty,
                    TagId = request.TagId,
                    NumberOfPadding = request.NumberOfPadding,
                    SeriesPatern = request.SeriesPattern ?? string.Empty,
                    isRepeating = request.IsRepeating,
                    IsActive = false,
                };

                await _writeUow.CheckInventory.AddAsync(newCheckInventory, cancellationToken);
            }

            var checkInventoryMappings = await GetCheckInventoryMappings(tag, cancellationToken);

            foreach (var mapping in checkInventoryMappings)
            {
                var newCheckInventory = new Captive.Data.Models.CheckInventory
                {
                    Id = Guid.NewGuid(),
                    CurrentSeries = request.StartingSeries,
                    StartingSeries = request.StartingSeries,
                    EndingSeries = request.EndingSeries,
                    WarningSeries = request.WarningSeries,
                    JsonMappingData = JsonConvert.SerializeObject(mapping),
                    TagId = request.TagId,
                    NumberOfPadding = request.NumberOfPadding,
                    SeriesPatern = request.SeriesPattern ?? string.Empty,
                    isRepeating = request.IsRepeating,
                    IsActive = true,
                };

                await _writeUow.CheckInventory.AddAsync(newCheckInventory, cancellationToken);
            }

            return;
        }

        public async Task<List<CheckInventoryMappingData>> GetCheckInventoryMappings (Tag tag, CancellationToken cancellationToken)
        {
            List<CheckInventoryMappingData> mappings = new List<CheckInventoryMappingData>();

            var branchIds = await _readUow.BankBranches.GetAll().Where(x => x.BankInfoId == tag.BankId).Select(x => x.Id).ToListAsync(cancellationToken);
            var productIds = await _readUow.Products.GetAll().Where(x => x.BankInfoId == tag.BankId).Select(x => x.Id).ToListAsync(cancellationToken);

            var formcheckIds = _readUow.FormChecks.GetAll().Include(x => x.Product).Where(x => x.Product.BankInfoId == tag.BankId ).Select(x => new Tuple<Guid, Guid>(x.Id, x.ProductId)).ToList();

            var includedBranch = new List<Guid>();
            var includedProducts = new List<Guid>();
            var includedFormCheck = new List<Tuple<Guid,Guid>>();

            if (tag.SearchByBranch)
            {
                if(tag.Mapping.Any(x => x.BranchId.HasValue))
                {
                    includedBranch = tag.Mapping.Where(x => x.BranchId.HasValue).Select(x => x.BranchId!.Value).ToList();
                }
                else
                {
                    includedBranch = branchIds;
                }
            }

            if (tag.SearchByProduct)
            {
                if (tag.Mapping.Any(x => x.ProductId.HasValue))
                {
                    includedProducts = tag.Mapping.Where(x => x.ProductId.HasValue).Select(x => x.ProductId!.Value).ToList();
                }
                else
                {
                    includedProducts = productIds;
                }
            }

            if (tag.SearchByFormCheck)
            {
                if (tag.Mapping.Any(x => x.FormCheckId.HasValue))
                {
                    var mappedFormChecks = tag.Mapping.Where(x => x.FormCheckId.HasValue).Select(x => x.FormCheckId!.Value).ToList();

                    includedFormCheck = formcheckIds.Where(x => mappedFormChecks.Contains(x.Item1)).ToList();
                }
                else
                {
                    includedFormCheck = formcheckIds;
                }
            }

            if (includedBranch.Any())
            {
                foreach (var branch in includedBranch) {

                    if (includedProducts.Any())
                    {
                        //If search by product is on
                        foreach (var product in includedProducts) {

                            if (includedFormCheck.Any())
                            {
                                foreach (var formCheck in includedFormCheck.Where(x => x.Item2 == product))
                                {
                                    mappings.Add(new CheckInventoryMappingData(new List<Guid> { branch }, new List<Guid> { product }, new List<Guid> { formCheck.Item1 }));
                                    continue;
                                }
                            }
                            else
                            {
                                mappings.Add(new CheckInventoryMappingData(new List<Guid> { branch }, new List<Guid> { product } , formcheckIds.Where(x =>x.Item2 == product).Select(x => x.Item1)));
                                continue;
                            }
                        }
                    }
                    else
                    {
                        //If search by product is off
                        if (includedFormCheck.Any())
                        {
                            foreach(var formCheck in includedFormCheck)
                            {
                                mappings.Add(new CheckInventoryMappingData(new List<Guid> { branch }, productIds, new List<Guid> { formCheck.Item1 }));
                                continue;
                            }
                        }
                        else
                        {
                            mappings.Add(new CheckInventoryMappingData(new List<Guid> { branch }, productIds, formcheckIds.Select(x => x.Item1)));
                            continue;
                        }
                    }
                
                }

                return mappings;
            }

            if (includedProducts.Any())
            {
                foreach (var product in includedProducts) {
                    if (includedFormCheck.Any(x => x.Item2 == product))
                    {
                        foreach (var formCheck in includedFormCheck.Where(x => x.Item2 == product))
                        {
                            mappings.Add(new CheckInventoryMappingData(branchIds , new List<Guid> { product }, new List<Guid> { formCheck.Item1 }));
                            continue;
                        }
                    }
                    else
                    {
                        mappings.Add(new CheckInventoryMappingData(branchIds, new List<Guid> { product }, formcheckIds.Where(x => x.Item2 == product).Select(x => x.Item1)));
                        continue;
                    }

                }
                return mappings;
            }

            if (includedFormCheck.Any())
            {
                foreach (var formCheck in includedFormCheck)
                {
                    mappings.Add(new CheckInventoryMappingData(branchIds, productIds.Where(x => x == formCheck.Item2), new List<Guid> { formCheck.Item1 }));
                    continue;
                }
            }

            return mappings;
        }
    }
}
