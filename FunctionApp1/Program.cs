using ClassLibrary1;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.FeatureManagement;
using Serilog;
using Serilog.Events;

const string contextValue = "Program";

//todo - choose a style that is easy to understand in development and production
//const string outputTemplate = "[{Level}] at [{Timestamp:HH:mm:ss.fff zzz}] on [{MachineName}] in [{SourceContext}] @ {Message}{NewLine}{Exception}";
const string outputTemplate = "{SourceContext} @ {Message}{NewLine}{Exception}";

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .WriteTo.Console(outputTemplate: outputTemplate)
    .CreateLogger();

Log.ForContext("SourceContext", contextValue).Information("Running Program");

//help - https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide
Log.ForContext("SourceContext", contextValue).Information("Creating Builder");
var builder = new HostBuilder().ConfigureFunctionsWorkerDefaults();

builder.ConfigureServices(s =>
{
    //todo - i guess this isn't needed as serilog is working
    //s.AddSingleton<ILogger>(Log.Logger);

    Log.ForContext("SourceContext", contextValue).Information("Adding Feature Management");
    s.AddFeatureManagement();

    Log.ForContext("SourceContext", contextValue).Information("Adding IService");
    s.AddSingleton<IService1, Service1>();
});

Log.ForContext("SourceContext", contextValue).Information("Building Application");
var app = builder.Build();

//app.Logger.LogInformation("Running Application");
await app.RunAsync();
