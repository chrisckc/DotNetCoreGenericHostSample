using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DotNetCoreGenericHostSample
{
    public class MyServiceX : BackgroundService
    {
        public MyServiceX(ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.CreateLogger<MyServiceX>();
            Logger.LogInformation("{0} : MyServiceX constructed.", DateTime.Now.ToString());
        }

        public ILogger Logger { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Logger.LogInformation("{0} : MyServiceX background task is starting.", DateTime.Now.ToString());

            stoppingToken.Register(() => Logger.LogInformation("{0} : MyServiceX is stopping.", DateTime.Now.ToString()));

            // Add a delay before first run to allow time for any database to first be created etc.
            //await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

            // Perform any required preparation here...

            while (!stoppingToken.IsCancellationRequested)
            {
                Logger.LogInformation("{0} : MyServiceX is doing background work.", DateTime.Now.ToString());

                //await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken).ContinueWith(task => { });
                await Task.Delay(TimeSpan.FromSeconds(5));
            }

            Logger.LogInformation("{0} : MyServiceX background task is stopping.", DateTime.Now.ToString());
            // Perform any required clean-up here...
            // Close database connections etc...
            await Task.Delay(TimeSpan.FromMilliseconds(1000));  // exaggerated to 1 second

            Logger.LogInformation("{0} : MyServiceX background task clean-up completed.", DateTime.Now.ToString());
        }
    }
}