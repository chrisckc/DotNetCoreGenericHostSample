using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DotNetCoreGenericHostSample
{
    // Same as MyServiceB but with cancellation added
    public class MyServiceC : IHostedService, IDisposable
    {
        private bool _stopping;
        private Task _backgroundTask;

        private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();

        public MyServiceC(ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.CreateLogger<MyServiceC>();
            Logger.LogInformation("{0} : MyServiceC constructed.", DateTime.Now.ToString());
        }

        public ILogger Logger { get; }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation("{0} : MyServiceC background task is starting.", DateTime.Now.ToString());
            _backgroundTask = BackgroundTask(_stoppingCts.Token);
            return Task.CompletedTask;
        }

        private async Task BackgroundTask(CancellationToken cancellationToken)
        {
            while (!_stopping)
            {
                Logger.LogInformation("{0} : MyServiceC is doing background work.", DateTime.Now.ToString());
                await Task.Delay(TimeSpan.FromSeconds(7), cancellationToken).ContinueWith(task => { });
                 //await Task.Delay(TimeSpan.FromSeconds(7));
                //Logger.LogInformation("MyServiceB is doing background work.");
            }

            Logger.LogInformation("{0} : MyServiceC background task is stopping.", DateTime.Now.ToString());
            // Perform any required clean-up here...
            // Close database connections etc...
            await Task.Delay(TimeSpan.FromMilliseconds(500)); 

            Logger.LogInformation("{0} : MyServiceC background task clean-up completed.", DateTime.Now.ToString());
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation("{0} : MyServiceC is stopping.", DateTime.Now.ToString());
            _stopping = true;
            if (_backgroundTask != null)
            {
                try
                {
                    // Signal cancellation to the executing method
                    Logger.LogInformation("{0} : calling _stoppingCts.Cancel()", DateTime.Now.ToString());
                    _stoppingCts.Cancel();
                    Logger.LogInformation("{0} : awaiting _backgroundTask", DateTime.Now.ToString());
                    await _backgroundTask;
                } catch(Exception ex) {
                    Logger.LogInformation("{0} : MyServiceC stopping Exception: {1}", DateTime.Now.ToString(), ex.Message);
                }
                finally {
                    Logger.LogInformation("{0} : MyServiceC stopped.", DateTime.Now.ToString());
                }
            }
        }

        public void Dispose()
        {
            Logger.LogInformation("{0} : MyServiceC is disposing.", DateTime.Now.ToString());
        }
    }
}