/*******************************************************************************
https://github.com/ronhowe
*******************************************************************************/

using ClassLibrary1;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.FeatureManagement;
using Serilog;
using Serilog.Events;

const string _sourceContext = nameof(Program);
const string _outputTemplate = "[{Timestamp:yyyy-mm-dd @ HH:mm:ss.fff}] [{SourceContext}] {Message}{NewLine}";
//const string _outputTemplate = "[{Timestamp:yyyy-mm-dd @ HH:mm:ss.fff}] [{Level:u3}] [{MachineName}] [{SourceContext}] {Message}{NewLine}{Exception}";

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .WriteTo.Console(outputTemplate: _outputTemplate)
    .CreateLogger();

#region post

/******************************************************************************

                    _
 _ __    ___   ___ | |_
| '_ \  / _ \ / __|| __|
| |_) || (_) |\__ \| |_
| .__/  \___/ |___/ \__|
|_|

******************************************************************************/

Log.ForContext("SourceContext", _sourceContext).Debug(PowerOnSelfTest.DebugLoggingOn);
Log.ForContext("SourceContext", _sourceContext).Information(PowerOnSelfTest.InformationLoggingOn);
Log.ForContext("SourceContext", _sourceContext).Warning(PowerOnSelfTest.WarningLoggingOn);
Log.ForContext("SourceContext", _sourceContext).Error(PowerOnSelfTest.ErrorLoggingOn);
Log.ForContext("SourceContext", _sourceContext).Fatal(PowerOnSelfTest.FatalLoggingOn);

#endregion post

Log.ForContext("SourceContext", _sourceContext).Information("Program Running");

//help - https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide
Log.ForContext("SourceContext", _sourceContext).Information("Creating Builder");
var builder = new HostBuilder().ConfigureFunctionsWorkerDefaults();

builder.ConfigureServices(s =>
{
    Log.ForContext("SourceContext", _sourceContext).Information("Adding Feature Management");
    s.AddFeatureManagement();

    Log.ForContext("SourceContext", _sourceContext).Information("Adding {0}", nameof(DateTimeService));
    s.AddSingleton<IDateTimeService, DateTimeService>();

    Log.ForContext("SourceContext", _sourceContext).Information("Adding {0}", nameof(Service1));
    s.AddSingleton<IService1, Service1>();
});

Log.ForContext("SourceContext", _sourceContext).Information("Building Application");
var app = builder.Build();

Log.ForContext("SourceContext", _sourceContext).Information("Awaiting Application");
await app.RunAsync();
