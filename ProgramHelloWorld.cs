using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace DotNetCoreGenericHostSample
{
    public class ProgramHelloWorld
    {
        public static async Task Main(string[] args)
        {
            //var builder = Host.CreateDefaultBuilder() // dotnet 3.0 only
            var builder = new HostBuilder()
                .ConfigureLogging((hostContext, logging) =>
                {
                    logging.AddConfiguration(hostContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                })
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    config.AddEnvironmentVariables();
                    config.AddJsonFile("appsettings.json", optional: true);
                    config.AddCommandLine(args);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    //services.AddHostedService<MyServiceA>();
                    //services.AddHostedService<MyServiceB>();
                    //services.AddHostedService<MyServiceC>();
                    services.AddHostedService<MyServiceX>();
                    services.AddHostedService<MyServiceY>();
                    services.AddHostedService<MyServiceZ>();
                });

            // dotnet 3.0 only
            // await builder.RunConsoleAsync(options =>
            // {
            //     options.SuppressStatusMessages = false;
            // });
            await builder.RunConsoleAsync();

        }
    }
}