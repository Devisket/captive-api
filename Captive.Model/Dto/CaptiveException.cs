using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captive.Model.Dto
{
    public class CaptiveException :Exception
    {
        public int StatusCode { get; }

        public CaptiveException(string message, int statusCode = 500) : base(message)
        {
            StatusCode = statusCode;
        }

        public CaptiveException(string message, Exception innerException, int statusCode = 500)
            : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }
}
