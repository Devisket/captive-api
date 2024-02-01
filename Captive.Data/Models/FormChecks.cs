using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captive.Data.Models
{
    public class FormChecks
    {
        public int Id { get; set; }
        
        public int FormTypeId { get; set; }
        public FormTypes FormType { get; set; }

        public int CheckTypeId { get; set; }
        public CheckTypes CheckType { get; set; }

        public string? Description { get; set; }
    }
}
