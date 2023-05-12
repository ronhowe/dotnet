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

    if (!builder.Environment.IsStaging())
    {

        builder.Services.AddAzureAppConfiguration();

        var connectionString = builder.Configuration.GetConnectionString("AzureAppConfiguration");

        builder.Configuration.AddAzureAppConfiguration(options =>
        {
            options
                .Connect(connectionString)
                //.Connect(new Uri(settings["AppConfig:Endpoint"]), new ManagedIdentityCredential())
                //.Connect(new Uri(settings["AppConfig:Endpoint"]), new DefaultAzureCredential(true))
                .ConfigureRefresh(refresh =>
                {
                    refresh.Register("sentinel", refreshAll: true)
                    .SetCacheExpiration(new TimeSpan(0, 0, 3));
                })
                .UseFeatureFlags(featureFlagOptions =>
                {
                    featureFlagOptions.CacheExpirationInterval = new TimeSpan(0, 0, 3);
                });
        });
    }

    builder.Services.AddScoped<IService1, Service1>();

    var app = builder.Build();

    if (!app.Environment.IsDevelopment() && !app.Environment.IsStaging())
    {
        app.UseAzureAppConfiguration();
    }

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
