using Captive.Model.Dto;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captive.Applications.TagAndMapping.Query.GetTagAndMapping
{
    public class GetTagAndMappingQuery : IRequest<TagDto>
    {
        public Guid Id { get; set; }
    }
}
