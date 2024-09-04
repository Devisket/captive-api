using Microsoft.Extensions.Hosting;

namespace Captive.Fileprocessor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

            IHost host = builder.Build();
            host.Run();
        }
    }
}
