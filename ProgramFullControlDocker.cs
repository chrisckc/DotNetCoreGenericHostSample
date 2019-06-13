using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DotNetCoreGenericHostSample
{
    public class ProgramFullControlDocker
    {
        public static async Task Main(string[] args)
        {
            var host = new HostBuilder()
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
                })
                .Build();

            var s = host.Services;

            using (host)
            {
                Console.WriteLine("Starting!");

                await host.StartAsync();

                //Console.WriteLine("Started! Press <enter> to stop.");
                Console.WriteLine("{0} : Started!", DateTime.Now.ToString());

                // Console.ReadLine();
                // Console.WriteLine("Stopping!");
                // await host.StopAsync();

                // Fixes issue in docker where any exception thrown is not in the container logs
                try {
                    await host.WaitForShutdownAsync();
                } catch(Exception ex) {
                    Console.WriteLine("{0} : Exception: {1}", DateTime.Now.ToString(), ex.ToString());
                }

                Console.WriteLine("{0} : Stopped!", DateTime.Now.ToString());
            }
        }
    }
}