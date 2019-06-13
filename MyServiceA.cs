using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DotNetCoreGenericHostSample
{
    public class MyServiceA : BackgroundService
    {
        public MyServiceA(ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.CreateLogger<MyServiceA>();
            Logger.LogInformation("{0} : MyServiceA constructed.", DateTime.Now.ToString());
        }

        public ILogger Logger { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Logger.LogInformation("{0} : MyServiceA background task is starting.", DateTime.Now.ToString());

            stoppingToken.Register(() => Logger.LogInformation("{0} : MyServiceA is stopping.", DateTime.Now.ToString()));

            // Add a delay before first run to allow time for any database to first be created etc.
            //await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

            // Perform any required preparation here...

            while (!stoppingToken.IsCancellationRequested)
            {
                Logger.LogInformation("{0} : MyServiceA is doing background work.", DateTime.Now.ToString());

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken).ContinueWith(task => { });
                //await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }

            Logger.LogInformation("{0} : MyServiceA background task is stopping.", DateTime.Now.ToString());
            // Perform any required clean-up here...
            // Close database connections etc...
            await Task.Delay(TimeSpan.FromMilliseconds(500)); 

            Logger.LogInformation("{0} : MyServiceA background task clean-up completed.", DateTime.Now.ToString());
        }
    }
}