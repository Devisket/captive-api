using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captive.Applications.Batch.Commands.CreateBatchFile
{
    public class CreateBatchFileCommand : IRequest<CreateBatchFileResponse>
    {
        public string? BatchName { get; set; }
        public Guid BankInfoId { get; set; }
        public DateTime DeliveryDate { get; set; }
    }

    public class CreateBatchFileResponse
    {
        public Guid Id { get; set; }

        public Guid BankInfoId { get; set; }
    }
}
