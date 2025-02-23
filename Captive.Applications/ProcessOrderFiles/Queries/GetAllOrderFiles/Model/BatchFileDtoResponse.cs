﻿
using Captive.Data.Enums;

namespace Captive.Applications.ProcessOrderFiles.Queries.GetAllOrderFiles.Model
{
    public  class BatchFileDtoResponse
    {
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public ICollection<OrderFileDtoResponse>? OrderFiles { get; set; }
    }
}
