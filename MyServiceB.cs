using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DotNetCoreGenericHostSample
{
    // Not inheriting from BackgroundService as MyServiceA does
    public class MyServiceB : IHostedService, IDisposable
    {
        private bool _stopping;
        private Task _backgroundTask;

        public MyServiceB(ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.CreateLogger<MyServiceB>();
            Logger.LogInformation("{0} : MyServiceB constructed.", DateTime.Now.ToString());
        }

        public ILogger Logger { get; }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation("{0} : MyServiceB background task is starting.", DateTime.Now.ToString());
            _backgroundTask = BackgroundTask();
            return Task.CompletedTask;
        }

        private async Task BackgroundTask()
        {
            while (!_stopping)
            {
                Logger.LogInformation("{0} : MyServiceB is doing background work.", DateTime.Now.ToString());
                await Task.Delay(TimeSpan.FromSeconds(7));
                //Logger.LogInformation("MyServiceB is doing background work.");
            }

            Logger.LogInformation("{0} : MyServiceB background task is stopping.", DateTime.Now.ToString());
            // Perform any required clean-up here...
            // Close database connections etc...
            await Task.Delay(TimeSpan.FromMilliseconds(500)); 

            Logger.LogInformation("{0} : MyServiceB background task clean-up completed.", DateTime.Now.ToString());
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation("{0} : MyServiceB is stopping.", DateTime.Now.ToString());
            _stopping = true;
            if (_backgroundTask != null)
            {
                // TODO: cancellation
                await _backgroundTask;
            }
        }

        public void Dispose()
        {
            Logger.LogInformation("{0} : MyServiceB is disposing.", DateTime.Now.ToString());
        }
    }
}