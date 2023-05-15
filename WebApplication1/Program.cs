using ClassLibrary1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using Serilog;
using Serilog.Events;
using WebApplication1;

// TODO - Optionally sync with appsettings.json for consistency in log message styling.
const string outputTemplate = "[{Level}] at [{Timestamp:HH:mm:ss.fff zzz}] on [{MachineName}] in [{SourceContext}] @ {Message}{NewLine}{Exception}";

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
    Log.ForContext("SourceContext", "Program").Information("Building Logger");
    var builder = WebApplication.CreateBuilder(args);

    Log.ForContext("SourceContext", "Program").Information("Using Serilog");
    builder.Host.UseSerilog((hostContext, LoggerConfiguration) =>
    {
        LoggerConfiguration.ReadFrom.Configuration(hostContext.Configuration);
    });

    Log.ForContext("SourceContext", "Program").Information("Adding Application Insights");
    builder.Services.AddApplicationInsightsTelemetry();

    Log.ForContext("SourceContext", "Program").Information("Adding Feature Management");
    builder.Services.AddFeatureManagement();

    Log.ForContext("SourceContext", "Program").Warning("TODO - Add Authorization");
    //builder.Services.AddAuthorization();

    Log.ForContext("SourceContext", "Program").Warning("TODO - Inject Configuration And Logging to Health Check");
    //https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-8.0#dependency-injection-and-health-checks
    builder.Services.AddHealthChecks().AddCheck<SampleHealthCheck>("Sample");

    if (builder.Environment.IsProduction())
    {
        Log.ForContext("SourceContext", "Program").Information("Adding Azure App Configuration");
        builder.Services.AddAzureAppConfiguration();

        Log.ForContext("SourceContext", "Program").Information("Getting Azure App Configuration Connection String");
        var connectionString = builder.Configuration.GetConnectionString("AzureAppConfiguration");

        Log.ForContext("SourceContext", "Program").Information("Configuring Azure App Configuration");
        builder.Configuration.AddAzureAppConfiguration(options =>
        {
            options
                // TODO - Confirm all of these work as expected and/or retire connectionString.
                .Connect(connectionString)
                //.Connect(new Uri(settings["AppConfig:Endpoint"]), new ManagedIdentityCredential())
                //.Connect(new Uri(settings["AppConfig:Endpoint"]), new DefaultAzureCredential(true))
                .ConfigureRefresh(refresh =>
                {
                    refresh.Register("sentinel", refreshAll: true)
                    //https://learn.microsoft.com/en-us/azure/azure-app-configuration/howto-best-practices#reduce-requests-made-to-app-configuration
                    .SetCacheExpiration(new TimeSpan(0, 1, 0));
                })
                .UseFeatureFlags(featureFlagOptions =>
                {
                    //https://learn.microsoft.com/en-us/azure/azure-app-configuration/howto-best-practices#reduce-requests-made-to-app-configuration
                    featureFlagOptions.CacheExpirationInterval = new TimeSpan(0, 1, 0);
                });
        });
    }
    else
    {
        Log.ForContext("SourceContext", "Program").Information("Skipping Azure App Configuration");
    }

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    Log.ForContext("SourceContext", "Program").Information("Adding IService");
    builder.Services.AddScoped<IService1, Service1>();

    Log.ForContext("SourceContext", "Program").Information("Building Application");
    var app = builder.Build();

    app.Logger.LogInformation("Environment = {EnvironmentName}", app.Environment.EnvironmentName);
    app.Logger.LogWarning("TODO - Log Pertinent Configuration Values?");

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // TODO - Reimplement For Non-Production Environment(s)
    if (app.Environment.IsProduction())
    {
        app.Logger.LogInformation("Using Azure App Configuration");
        app.UseAzureAppConfiguration();
    }
    else
    {
        app.Logger.LogInformation("Not Using Azure App Configuration");
    }

    // TODO - Refactor to ICustomHeader.
    app.Logger.LogInformation("Using Custom Header Lamda");
    app.Use(async (context, next) =>
    {
        //https://code-maze.com/aspnetcore-add-custom-headers/
        const string headerKey = "CustomHeader";
        var headerValue = app.Configuration.GetSection(headerKey).Value;
        app.Logger.LogDebug("Adding Custom Header {headerKey}={headerValue}", headerKey, headerValue);
        context.Response.Headers.Add(headerKey, headerValue);
        await next();
    });

    app.Logger.LogInformation("Using Serilog Request Logging");
    app.UseSerilogRequestLogging();

    app.Logger.LogWarning("TODO - Implement HTTPS Redirection");
    //app.UseHttpsRedirection();

    app.Logger.LogWarning("TODO - Review Feature Requirements Per Environment");
    if (!app.Environment.IsDevelopment())
    {
        app.Logger.LogWarning("TODO - Implement Exception Handler");
        //app.UseExceptionHandler("/error");

        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.Logger.LogWarning("TODO - Implement HSTS");
        //app.UseHsts();
    }
    else
    {
        app.Logger.LogWarning("TODO - Implement Else Condition");
    }

    app.Logger.LogWarning("TODO - Implement Health Checks");
    //app.UseHealthChecks(ApplicationEndpoint.PowerOnSelfTest);
    app.UseHealthChecks(ApplicationEndpoint.HealthCheck);

    app.Logger.LogWarning("TODO - Implement Authentication");
    //app.UseAuthentication();

    app.Logger.LogWarning("TODO - Implement Authorization");
    //app.UseAuthorization();

    app.Logger.LogInformation("Mapping Get");
    app.Logger.LogWarning("TODO - Implement Endpoint Constants");
    app.MapGet(ApplicationEndpoint.Service1, (/*[FromRoute]*/ bool? input, [FromServices] IService1 service) =>
    {
        app.Logger.LogWarning("TODO - Implement Identity And Claims Services");
        // TODO - Example Code And Comments
        //httpContext.ValidateAppRole(ApplicationRole.CanRead);
        //public static class ApplicationRole
        //{
        //    public const string CanRead = "DaemonAppRole"; // TODO Line up with ARM Deployment
        //    public const string CanWrite = "DataWriterRole"; // TODO Line up with ARM Deployment
        //}
        return service.Run(input);
    });
    app.Logger.LogDebug("TODO - Implement Authorization");
    //.RequireAuthorization();

    app.Logger.LogInformation("Running Application");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Program Exception");
}
finally
{
    Log.CloseAndFlush();
}
