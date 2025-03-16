

using Captive.Model.Enums;

namespace Captive.Model.Dto
{
    public class LogDto
    {
        public string LogMessage { get; set; } = string.Empty;
        public LogType LogType { get; set; }
    }
}
