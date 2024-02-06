using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captive.Data.Models
{
    public class Seeds
    {

        public required int Id { get; set; }
        public required string SeedName { get; set; }

        public required DateTime SeedDate {  get; set; }
    }
}
