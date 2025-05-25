using Captive.Data.Enums;
using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using Captive.Model.Application;
using Captive.Model.Dto;
using Captive.Utility;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Captive.Applications.TagAndMapping.Command.CreateMapping
{
    public class CreateTagMappingCommandHandler : IRequestHandler<CreateTagMappingCommand, Unit>
    {

        private readonly IReadUnitOfWork _readUow;
        private readonly IWriteUnitOfWork _writeUow;

        public CreateTagMappingCommandHandler(IReadUnitOfWork readUow, IWriteUnitOfWork writeUow)
        {
            _readUow = readUow;
            _writeUow = writeUow;
        }

        public async Task<Unit> Handle(CreateTagMappingCommand request, CancellationToken cancellationToken)
        {
            var tag = await _readUow.Tags.GetAll().FirstOrDefaultAsync(x => x.Id == request.TagId, cancellationToken);

            if (tag == null)
                throw new Exception($"The Tag ID: {request.TagId} doesn't exist");

            if (!await IsRequestValid(request, tag.BankId, cancellationToken))
                throw new CaptiveException("Request is invalid");
            
            await CreateTagMapping(request, cancellationToken);
            
            return Unit.Value;
        }

        private async Task CreateTagMapping(CreateTagMappingCommand request, CancellationToken cancellationToken)
        {
            var mappingData = JsonConvert.SerializeObject(GetMappingData(request));

            if (request.Id.HasValue)
            {
                var tagMapping = await _readUow.TagsMapping.GetAll().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (tagMapping == null)
                    throw new CaptiveException($"Tag Mapping ID: {request.Id} doesn't exist.");

                tagMapping.TagMappingData = mappingData;
                tagMapping.UpdatedDate = DateTime.UtcNow;

                _writeUow.TagMappings.Update(tagMapping);
            }
            else
            {
                await _writeUow.TagMappings.AddAsync(new TagMapping
                {
                    Id = Guid.NewGuid(),
                    TagId = request.TagId,
                    TagMappingData = mappingData,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow,

                }, cancellationToken);
            }
        }

        private async Task<bool> IsRequestValid(CreateTagMappingCommand request, Guid bankId, CancellationToken cancellationToken)
        {
            var branchs = await _readUow.BankBranches.GetAll().AsNoTracking().Where(x => x.BankInfoId == bankId).Select(x => x.Id).ToListAsync(cancellationToken);
            var products = await _readUow.Products.GetAll().AsNoTracking().Where(x => x.BankInfoId == bankId).Select(x => x.Id).ToListAsync(cancellationToken);

            if (request.MappingData.BranchIds == null || request.MappingData.BranchIds.Any(x => !branchs.Contains(x)))
                return false;

            if(request.MappingData.ProductIds != null && request.MappingData.ProductIds.Count() > 0)
                if (request.MappingData.ProductIds.Any(x => !products.Contains(x)))
                    return false;

            var existingTagMapping = _readUow.TagsMapping.GetAll()
                .Include(x => x.Tag)
                .Where(tagMapping => tagMapping.Tag.BankId == bankId && !tagMapping.Tag.isDefaultTag)
                .Select(tagMapping => JsonConvert.DeserializeObject<TagMappingData>(tagMapping.TagMappingData))
                .ToList();

            //I want to check every mapping if the combination of one of my mapping is already exist.
            //Scenario
            //Other mapping has: Branch A, B C | Product A, B | Formcheck A, B
            //New mapping has: Branch A, D | Product B | Formcheck A 
            //At this point combination of Branch A - Product B - FormCheck A both exist into these mappings 

            if(existingTagMapping.Any(mapping => mapping.BranchIds.ContainsAny(request.MappingData.BranchIds) && mapping.ProductIds.ContainsAny(request.MappingData.ProductIds) && mapping.FormCheckType.ContainsAny(request.MappingData.FormCheckType)))
            {
                throw new CaptiveException("There is conflicted mapping");
            }

            return true;
        }

        private TagMappingData GetMappingData(CreateTagMappingCommand request)
        {
            return new TagMappingData
            {
                BranchIds = request.MappingData.BranchIds,
                ProductIds = request.MappingData.ProductIds,
                FormCheckType = request.MappingData.FormCheckType
            };
        }
    }
}
