using Captive.Processing.Processor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captive.Processing.Processor
{
    public interface IFileProcessor
    {
        public ICollection<OrderFileData> OnProcessFile(byte[] file, string orderFileConfiguration);
    }
}
