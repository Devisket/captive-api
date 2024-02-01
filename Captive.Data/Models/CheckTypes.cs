using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captive.Data.Models
{
    public class CheckTypes
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Value { get; set; }
        public string? Description { get; set; }

        public int BankId { get; set; }
        public required BankInfo Bank { get; set; }

        public ICollection<FormChecks>? FormChecks { get; set; }
    }
}
