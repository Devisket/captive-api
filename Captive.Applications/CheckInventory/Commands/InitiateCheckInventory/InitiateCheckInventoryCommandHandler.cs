using Captive.Data.Enums;
using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using Captive.Model.Application;
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

            tag.CheckInventoryInitiated = true;

            return;
        }

        public async Task<List<CheckInventoryMappingData>> GetCheckInventoryMappings (Tag tag, CancellationToken cancellationToken)
        {
            List<CheckInventoryMappingData> mappings = new List<CheckInventoryMappingData>();

            var branchIds = await _readUow.BankBranches.GetAll().Where(x => x.BankInfoId == tag.BankId).Select(x => x.Id).ToListAsync(cancellationToken);
            var productIds = await _readUow.Products.GetAll().Where(x => x.BankInfoId == tag.BankId).Select(x => x.Id).ToListAsync(cancellationToken);

            var includedBranch = new List<Guid>();
            var includedProducts = new List<Guid>();
            var includedFormCheck = new List<FormCheckType>();

            var mappingDatas = tag.isDefaultTag?  null  : tag.Mapping?.Where(x => !string.IsNullOrEmpty(x.TagMappingData)).Select(x => JsonConvert.DeserializeObject<TagMappingData>(x.TagMappingData)).ToList();

            if (tag.SearchByBranch)
            {
                if(mappingDatas != null && mappingDatas.Any(x => x!.BranchIds.Any()))
                    includedBranch = mappingDatas.Where(x => x!.BranchIds.Any()).SelectMany(x => x!.BranchIds).Distinct().ToList();
                else
                    includedBranch = branchIds;
            }

            if (tag.SearchByProduct)
            {
                if (mappingDatas != null && mappingDatas.Any(x => x!.ProductIds.Any()))
                    includedProducts = mappingDatas.Where(x => x!.ProductIds.Any()).SelectMany(x => x!.ProductIds).Distinct().ToList();
                else
                    includedProducts = productIds;
            }

            if (tag.SearchByFormCheck)
            {
                if (mappingDatas != null && mappingDatas.Any(x => x!.FormCheckType.Any()))
                    includedFormCheck = mappingDatas.Where(x => x!.FormCheckType.Any()).SelectMany(x => x!.FormCheckType.Select(z=> Enum.Parse<FormCheckType>(z))).Distinct().ToList();
                else
                    includedFormCheck = new List<FormCheckType> { FormCheckType.Personal, FormCheckType.Commercial };
            }

            if (includedBranch.Any())
            {
                foreach (var branch in includedBranch) {

                    if (includedProducts.Any())
                    {
                        foreach (var product in includedProducts) {

                            if (includedFormCheck.Any())
                            {
                                foreach (var formCheck in includedFormCheck)
                                {
                                    mappings.Add(new CheckInventoryMappingData(new List<Guid> { branch }, new List<Guid> { product }, new List<string> { formCheck.ToString()}));
                                    continue;
                                }
                            }
                            else
                            {
                                mappings.Add(new CheckInventoryMappingData(new List<Guid> { branch }, new List<Guid> { product } , new List<string>(){ FormCheckType.Personal.ToString(), FormCheckType.Commercial.ToString() }));
                                continue;
                            }
                        }
                    }
                    else
                    {
                        if (includedFormCheck.Any())
                        {
                            foreach(var formCheck in includedFormCheck)
                            {
                                mappings.Add(new CheckInventoryMappingData(new List<Guid> { branch }, productIds, new List<string> { formCheck.ToString() }));
                                continue;
                            }
                        }
                        else
                        {
                            mappings.Add(new CheckInventoryMappingData(new List<Guid> { branch }, productIds, new List<string>() { FormCheckType.Personal.ToString(), FormCheckType.Commercial.ToString()}));
                            continue;
                        }
                    }
                }
                return mappings;
            }

            if (includedProducts.Any())
            {
                foreach (var product in includedProducts) {
                    if (includedFormCheck.Any())
                    {
                        foreach (var formCheck in includedFormCheck)
                        {
                            mappings.Add(new CheckInventoryMappingData(branchIds , new List<Guid> { product }, new List<string> { formCheck.ToString() }));
                            continue;
                        }
                    }
                    else
                    {
                        mappings.Add(new CheckInventoryMappingData(branchIds, new List<Guid> { product }, new List<string>() { FormCheckType.Commercial.ToString(), FormCheckType.Personal.ToString() }));
                        continue;
                    }

                }
                return mappings;
            }

            if (includedFormCheck.Any())
            {
                foreach (var formCheck in includedFormCheck)
                {
                    mappings.Add(new CheckInventoryMappingData(branchIds, productIds, new List<string> { formCheck.ToString() }));
                    continue;
                }
            }

            return mappings;
        }
    }
}
