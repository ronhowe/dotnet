using ClassLibrary1;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.FeatureManagement;
using Serilog;
using Serilog.Events;

namespace FunctionApp;

class Program
{
#pragma warning disable IDE0060 // Remove unused parameter
    static async Task Main(string[] args)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        const string contextValue = "Program";

        const string outputTemplate = "{SourceContext} @ {Message}{NewLine}{Exception}";

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .WriteTo.Console(outputTemplate: outputTemplate)
            .CreateLogger();

        Log.ForContext("SourceContext", contextValue).Information("Running Program");

        var host = new HostBuilder()
            .ConfigureFunctionsWorkerDefaults(builder =>
            {
                builder.Services.AddFeatureManagement();
            })
            .ConfigureServices(s =>
            {
                s.AddSingleton<IService1, Service1>();
            })
            .Build();

        await host.RunAsync();
    }
}