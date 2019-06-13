using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DotNetCoreGenericHostSample
{
    public class MyServiceY : BackgroundService
    {
        public MyServiceY(ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.CreateLogger<MyServiceY>();
            Logger.LogInformation("{0} : MyServiceY constructed.", DateTime.Now.ToString());
        }

        public ILogger Logger { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Logger.LogInformation("{0} : MyServiceY background task is starting.", DateTime.Now.ToString());

            stoppingToken.Register(() => Logger.LogInformation("{0} : MyServiceY is stopping.", DateTime.Now.ToString()));

            // Add a delay before first run to allow time for any database to first be created etc.
            //await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

            // Perform any required preparation here...

            while (!stoppingToken.IsCancellationRequested)
            {
                Logger.LogInformation("{0} : MyServiceY is doing background work.", DateTime.Now.ToString());
                
                //await Task.Delay(TimeSpan.FromSeconds(5.1), stoppingToken).ContinueWith(task => { });
                await Task.Delay(TimeSpan.FromSeconds(5.1));
            }

            Logger.LogInformation("{0} : MyServiceY background task is stopping.", DateTime.Now.ToString());
            // Perform any required clean-up here...
            // Close database connections etc...
            await Task.Delay(TimeSpan.FromMilliseconds(1000)); // exaggerated to 1 second

            Logger.LogInformation("{0} : MyServiceY background task clean-up completed.", DateTime.Now.ToString());
        }
    }
}