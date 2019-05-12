using Kungshol.Services.PowerLinux.Controllers;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using Serilog.Core.Sinks;

namespace Kungshol.Services.PowerLinux
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Logger logger = new LoggerConfiguration()
                .WriteTo.Seq("http://172.16.1.11:5341")
                .WriteTo.Console()
                .MinimumLevel.Verbose()
                .CreateLogger();

            CreateWebHostBuilder(args, logger).Build().Run();

            logger.Dispose();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args, ILogger logger)
        {
            IWebHostBuilder webHostBuilder = WebHost.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddSingleton(logger);
                    services.AddSingleton<UpsStatusService>();
                })
                .UseSerilog(logger)
                .UseKestrel(options => options.ListenAnyIP(5000))
                .UseStartup<Startup>();

            return webHostBuilder;
        }
    }
}