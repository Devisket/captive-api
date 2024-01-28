using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captive.Processing.Processor
{
    public interface IFileProcessor
    {
        public void OnProcessFile(byte[] file);
    }
}
