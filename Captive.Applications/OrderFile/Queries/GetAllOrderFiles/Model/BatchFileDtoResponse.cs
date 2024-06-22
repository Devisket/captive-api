﻿
using Captive.Data.Enums;

namespace Captive.Applications.OrderFile.Queries.GetAllOrderFiles.Model
{
    public  class BatchFileDtoResponse
    {
        public Guid Id { get; set; }
        public DateTime UploadDate { get; set; }
        public ICollection<OrderFileDtoResponse>? OrderFiles { get; set; }
    }
}
