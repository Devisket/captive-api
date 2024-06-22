﻿using Captive.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace Captive.Data.Models
{
    public class OrderFile
    {
        public Guid Id { get; set; }
        public required string FileName { get; set; }
        public required OrderFilesStatus Status { get; set; }
        public DateTime ProcessDate { get; set; }
        public ICollection<CheckOrders>? CheckOrders { get; set; }
        public ICollection<OrderFileLog>? OrderFileLogs { get; set; }

        public Guid BatchFileId { get; set; }
        public BatchFile? BatchFile { get; set; }
    }
}
