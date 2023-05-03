using ClassLibrary1;
using Microsoft.FeatureManagement;
using Serilog;
using Serilog.Events;

// sync with appsettings.json
const string outputTemplate = "[{Level}] at [{Timestamp:HH:mm:ss.fff zzz}] on [{MachineName}] in [{SourceContext}]{NewLine}    {Message}{NewLine}{Exception}";

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .WriteTo.Console(outputTemplate: outputTemplate)
    .CreateLogger();

Log.ForContext("SourceContext", "Program").Information("Running Program");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((hostContext, LoggerConfiguration) =>
    {
        LoggerConfiguration.ReadFrom.Configuration(hostContext.Configuration);
    });

    builder.Services.AddApplicationInsightsTelemetry();

    builder.Services.AddFeatureManagement();

    builder.Services.AddScoped<IService1, Service1>();

    var app = builder.Build();

    app.Use(async (context, next) =>
    {
        //https://code-maze.com/aspnetcore-add-custom-headers/
        const string headerKey = "CustomHeader";
        var headerValue = app.Configuration.GetSection(headerKey).Value;
        app.Logger.LogDebug("Adding Header {headerKey}={headerValue}", headerKey, headerValue);
        context.Response.Headers.Add(headerKey, headerValue);
        await next();
    });

    app.UseSerilogRequestLogging();

    app.MapGet("/", (bool? input, IService1 service) =>
    {
        return service.Run(input);
    });

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Fatal Error");
}
finally
{
    Log.CloseAndFlush();
}
