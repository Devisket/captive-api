using Captive.Processing.Processor.TextFileProcessor;
using Microsoft.Extensions.Configuration;

namespace Captive.TestProcessingConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, true);
                       
            var fileProcessor = new TextFileProcessor(builder.Build());

            fileProcessor.OnProcessFile(File.ReadAllBytes("C:\\Users\\ediso\\repository\\personal\\SBTC\\Upload files\\CPTIVE0112.txt"));
        }
    }
}
