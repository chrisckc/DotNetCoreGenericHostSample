# ASP.NET Core GenericHostSample

This is based on the latest code from here:
https://github.com/aspnet/Extensions/tree/master/src/Hosting/samples/GenericHostSample

### Changes

It has been modified to work with dotnet core 2.2 as the above repo is setup for 3.0

ProgramHelloWorld.cs has been modified to work with 2.2

References in the csproj file have been updated to run against the installed dotnet framework.

Removed the custom DI container stuff.

References to EventLog and WindowsServices removed.

The StartupObject in the csproj file has been set to ProgramHelloWorld.

ProgramFullControl modified to use WaitForShutdownAsync()

Unused code lines from the sample have been left in but commented out.

Logging has been configured properly to allow the log messages to be seen.

VSCode debug setup added.

### Improvements

Modified logging to include timestamps and added some extra logging to show the sequence of events when performing a Ctrl-C to stop the process.


I have also added MyServiceC, similar to MyServiceB to demonstrate HostedService without inheriting from BackgroundTask but allowing cancellation, it is more or less doing what is already done by the BackgroundService base class.

Added MyServiceX, MyServiceY, and MyServiceZ to demonstrate running multiple concurrent HostedServices inheriting from BackgroundTask.

Added Docker config for testing the starting and stopping of a container running HostedServices.

In a real life situation it would be useful to be able to specify a global delay in starting all of the HostedServices after the GenericHost is started up. For example in a docker container scenario, a database container might not be ready by the time one of the hosted services needs to access the database etc. some example code has been included in the MyService implementations which simply adds a Task.Delay before starting the Task loop.

### Issues?

#### Issue 1

The sample code does not demonstrate the usage very well, for example using MyServiceA it is not possible to run any clean-up code after

```await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);```

This is because cancelling a Task raises an Exception, so if this was a real world Task it would need to be wrapped in a try catch or use .ContinueWith on the Task to swallow the Exception so that any clean-up code can run.

```await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken).ContinueWith(task => { });```

#### Issue 2

When using MyServiceX, MyServiceY, and MyServiceZ concurrently, the services are stopped in sequence when Ctrl-C is pressed, rather than concurrently.
Not sure if this is the intended behaviour as it results in a situation where MyServiceZ announces it is stopping and then MyServiceX and MyServiceY announce that they are doing background work in a new cycle.
Despite the above it works fine as long as the long running tasks can be cancelled with CancellationToken, for example:

```await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken).ContinueWith(task => { });```

However if the long running task does not accept a CancellationToken such as:

```await Task.Delay(TimeSpan.FromSeconds(5));```

Notice that MyService Z stops correctly but MyServiceY and MyServiceX do not stop properly because an exception is raised during the stopping of the second service (whichever service is next in line to be stopped).
If there is only a single service there is no issue with Task that can't be cancelled.

Also notice that when running in Docker, the same issue occurs but the exception is not visible in the docker logs.

```
Unhandled Exception: System.OperationCanceledException: The operation was canceled.
   at System.Threading.CancellationToken.ThrowOperationCanceledException()
   at Microsoft.Extensions.Hosting.Internal.Host.StopAsync(CancellationToken cancellationToken)
   at Microsoft.Extensions.Hosting.HostingAbstractionsHostExtensions.WaitForShutdownAsync(IHost host, CancellationToken token)
   at Microsoft.Extensions.Hosting.HostingAbstractionsHostExtensions.RunAsync(IHost host, CancellationToken token)
   at DotNetCoreGenericHostSample.ProgramHelloWorld.Main(String[] args) in /Users/.........../DotNetCoreGenericHostSample/ProgramHelloWorld.cs:line 42
   at DotNetCoreGenericHostSample.ProgramHelloWorld.<Main>(String[] args)
```

### Debugging

Debug in VSCode has been configured.

Note: Stopping the debug session seems to completely kill the process and none of the shutdown code executes.

### Running

To see what happens the whe process is stopped:

```export ASPNETCORE_ENVIRONMENT=Development```

```dotnet run -p DotNetCoreGenericHostSample.csproj```

Then use Ctrl-C to stop it.

### Docker

```docker-compose -up -d```

once started, wait about 20 seconds for some logs to build up, then run:

```docker-compose stop```

then check the logs using:

 ```docker logs dotnetcoregenerichostsample_dotnetcoregenerichostsample_1```

Notice the same issue when using MyServiceX, MyServiceY, and MyServiceZ concurrently and Service Y and X are not stopped properly, only this time the Exception is not shown in the docker logs whcih is a bad thing.

This can be resolved in the ProgramFullControl by wrapping WaitForShutdownAsync() in a try catch and writing the exception to the console, however the same technique does not work for ProgramHelloWorld (wrapping RunConsoleAsync(); in a try catch).
Set the Startup object in the csproj file to ProgramFullControlDocker or ProgramHelloWorldDocker to test this.
