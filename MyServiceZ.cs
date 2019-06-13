using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DotNetCoreGenericHostSample
{
    public class MyServiceZ : BackgroundService
    {
        public MyServiceZ(ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.CreateLogger<MyServiceZ>();
            Logger.LogInformation("{0} : MyServiceZ constructed.", DateTime.Now.ToString());
        }

        public ILogger Logger { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Logger.LogInformation("{0} : MyServiceZ background task is starting.", DateTime.Now.ToString());

            stoppingToken.Register(() => Logger.LogInformation("{0} : MyServiceZ is stopping.", DateTime.Now.ToString()));

            // Add a delay before first run to allow time for any database to first be created etc.
            //await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

            // Perform any required preparation here...

            while (!stoppingToken.IsCancellationRequested)
            {
                Logger.LogInformation("{0} : MyServiceZ is doing background work.", DateTime.Now.ToString());
                
                //await Task.Delay(TimeSpan.FromSeconds(5.2), stoppingToken).ContinueWith(task => { });
                await Task.Delay(TimeSpan.FromSeconds(5.2));
            }

            Logger.LogInformation("{0} : MyServiceZ background task is stopping.", DateTime.Now.ToString());
            // Perform any required clean-up here...
            // Close database connections etc...
            await Task.Delay(TimeSpan.FromMilliseconds(1000)); // exaggerated to 1 second

            Logger.LogInformation("{0} : MyServiceZ background task clean-up completed.", DateTime.Now.ToString());
        }
    }
}