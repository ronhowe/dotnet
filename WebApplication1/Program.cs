/////////////////////////////////////////////////////////////////////////////80

using ClassLibrary1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using Serilog;
using Serilog.Events;
using WebApplication1;
//using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;


const string sourceContext = nameof(Program);
const string outputTemplate = "[SERVER] [{Timestamp:HH:mm:ss.fff zzz}] [{MachineName}] [{Level:u3}] [{SourceContext}] [{Message}]{NewLine}{Exception}";

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .WriteTo.Console(outputTemplate: outputTemplate)
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

Log.ForContext("SourceContext", sourceContext).Debug("Power-On Self-Test (1 of 5)... OK");
Log.ForContext("SourceContext", sourceContext).Information("Power-On Self-Test (2 of 5)... OK");
Log.ForContext("SourceContext", sourceContext).Warning("Power-On Self-Test (3 of 5)... OK");
Log.ForContext("SourceContext", sourceContext).Error("Power-On Self-Test (4 of 5)... OK");
Log.ForContext("SourceContext", sourceContext).Fatal("Power-On Self-Test (5 of 5)... OK");

#endregion post

Log.ForContext("SourceContext", sourceContext).Information("Program Running");

try
{
    Log.ForContext("SourceContext", sourceContext).Information("Creating Web Application Builder");
    var builder = WebApplication.CreateBuilder(args);

    #region logging

    /******************************************************************************

     _                       _
    | |  ___    __ _   __ _ (_) _ __    __ _
    | | / _ \  / _` | / _` || || '_ \  / _` |
    | || (_) || (_| || (_| || || | | || (_| |
    |_| \___/  \__, | \__, ||_||_| |_| \__, |
               |___/  |___/            |___/

    ******************************************************************************/

    Log.ForContext("SourceContext", sourceContext).Information("Using Serilog");
    builder.Host.UseSerilog((hostContext, LoggerConfiguration) =>
    {
        LoggerConfiguration.ReadFrom.Configuration(hostContext.Configuration);
    });

    #endregion logging

    #region services

    /******************************************************************************

                             _
     ___   ___  _ __ __   __(_)  ___   ___  ___
    / __| / _ \| '__|\ \ / /| | / __| / _ \/ __|
    \__ \|  __/| |    \ V / | || (__ |  __/\__ \
    |___/ \___||_|     \_/  |_| \___| \___||___/

     ******************************************************************************/

    //help - configure services (order doesn't matter unless you "materialize dependencies")
    //todo - learn what that means
    //link - https://www.youtube.com/watch?v=pYl_jnqlXu8

    Log.ForContext("SourceContext", sourceContext).Information("Adding Application Insights");
    builder.Services.AddApplicationInsightsTelemetry();

    Log.ForContext("SourceContext", sourceContext).Information("Adding Feature Management");
    builder.Services.AddFeatureManagement();

    //todo - add authorization
    //builder.Services.AddAuthorization();

    //todo - inject configuration and logging to health check
    //link - https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-8.0#dependency-injection-and-health-checks
    builder.Services.AddHealthChecks().AddCheck<Service1HealthCheck>("Sample");

    if (builder.Environment.IsProduction())
    {
        Log.ForContext("SourceContext", sourceContext).Information("Adding Azure App Configuration");
        builder.Services.AddAzureAppConfiguration();

        Log.ForContext("SourceContext", sourceContext).Information("Getting Azure App Configuration Connection String");
        var connectionString = builder.Configuration.GetConnectionString("AzureAppConfiguration");

        Log.ForContext("SourceContext", sourceContext).Information("Configuring Azure App Configuration");
        builder.Configuration.AddAzureAppConfiguration(options =>
        {
            options
                //todo - confirm all of these work as expected and/or retire connectionstring
                .Connect(connectionString)
                //.Connect(new Uri(settings["AppConfig:Endpoint"]), new ManagedIdentityCredential())
                //.Connect(new Uri(settings["AppConfig:Endpoint"]), new DefaultAzureCredential(true))
                .ConfigureRefresh(refresh =>
                {
                    refresh.Register("sentinel", refreshAll: true)
                    //help - https://learn.microsoft.com/en-us/azure/azure-app-configuration/howto-best-practices#reduce-requests-made-to-app-configuration
                    .SetCacheExpiration(new TimeSpan(0, 1, 0));
                })
                .UseFeatureFlags(featureFlagOptions =>
                {
                    //help - https://learn.microsoft.com/en-us/azure/azure-app-configuration/howto-best-practices#reduce-requests-made-to-app-configuration
                    featureFlagOptions.CacheExpirationInterval = new TimeSpan(0, 1, 0);
                });
        });
    }
    else
    {
        Log.ForContext("SourceContext", sourceContext).Information("Skipping Azure App Configuration");
    }

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    Log.ForContext("SourceContext", sourceContext).Information("Adding {0}", nameof(Service1));
    builder.Services.AddSingleton<IService1, Service1>();

    #endregion services

    #region build

    /******************************************************************************

     _             _  _      _
    | |__   _   _ (_)| |  __| |
    | '_ \ | | | || || | / _` |
    | |_) || |_| || || || (_| |
    |_.__/  \__,_||_||_| \__,_|

     ******************************************************************************/

    Log.ForContext("SourceContext", sourceContext).Information("Building Web Application");
    var app = builder.Build();

    app.Logger.LogDebug("Power-On Self-Test (1 of 5)... OK");
    app.Logger.LogInformation("Power-On Self-Test (2 of 5)... OK");
    app.Logger.LogWarning("Power-On Self-Test (3 of 5)... OK");
    app.Logger.LogError("Power-On Self-Test (4 of 5)... OK");
    app.Logger.LogCritical("Power-On Self-Test (5 of 5)... OK");

    app.Logger.LogInformation("Web Application Running");

    #endregion build

    #region configuration

    /******************************************************************************

                          __  _                             _    _
      ___   ___   _ __   / _|(_)  __ _  _   _  _ __   __ _ | |_ (_)  ___   _ __
     / __| / _ \ | '_ \ | |_ | | / _` || | | || '__| / _` || __|| | / _ \ | '_ \
    | (__ | (_) || | | ||  _|| || (_| || |_| || |   | (_| || |_ | || (_) || | | |
     \___| \___/ |_| |_||_|  |_| \__, | \__,_||_|    \__,_| \__||_| \___/ |_| |_|
                                 |___/

     ******************************************************************************/

    //help - order matters (e.g. add swagger before auth)

    app.Logger.LogInformation("Using Request Logging Middleware");
    app.UseMiddleware<RequestLoggingMiddleware>();

    //todo - log pertinent configuration values
    app.Logger.LogInformation("Environment = {EnvironmentName}", app.Environment.EnvironmentName);

    if (app.Environment.IsDevelopment())
    {
        app.Logger.LogInformation("Using Swagger");
        app.UseSwagger();
    }
    else
    {
        app.Logger.LogInformation("Skipping Swagger");
    }

    if (app.Environment.IsDevelopment())
    {
        app.Logger.LogInformation("Using Swagger UI");
        app.UseSwagger();
    }
    else
    {
        app.Logger.LogInformation("Using Swagger UI");
        app.UseSwaggerUI();
    }

    //todo - reimplement for non-production environment(s)
    if (app.Environment.IsProduction())
    {
        app.Logger.LogInformation("Using Azure App Configuration");
        app.UseAzureAppConfiguration();
    }
    else
    {
        app.Logger.LogInformation("Skipping Azure App Configuration");
    }


    //todo - refactor to icustomheader
    app.Logger.LogInformation("Using Custom Header");
    app.Use(async (context, next) =>
    {
        AddCustomHeader(context, app);
        await next();
    });

    app.Logger.LogInformation("Using Serilog Request Logging");
    app.UseSerilogRequestLogging();

    //todo - implement https redirection
    //app.UseHttpsRedirection();

    //todo - review feature requirements per environment
    if (!app.Environment.IsDevelopment())
    {
        //todo - implement exception handler
        //app.UseExceptionHandler("/error");

        //todo - implement hsts
        //link - https://aka.ms/aspnetcore-hsts
        //app.UseHsts();
    }
    else
    {
        //todo - implement else condition
    }

    app.Logger.LogInformation("Using HealthCheck");
    app.UseHealthChecks(Service1Endpoint.HealthCheck);

    //todo - implement authentication
    //app.UseAuthentication();

    //todo - implement authorization
    //app.UseAuthorization();

    /*

                         _
     _ __   ___   _   _ | |_   ___  ___
    | '__| / _ \ | | | || __| / _ \/ __|
    | |   | (_) || |_| || |_ |  __/\__ \
    |_|    \___/  \__,_| \__| \___||___/

     */

    app.Logger.LogInformation("Mapping Routes");
    app.MapGet(Service1Endpoint.Service1, (/*[FromRoute]*/ bool input, [FromServices] IService1 service) =>
    {
        //todo - implement identity and claims services
        /*
        httpContext.ValidateAppRole(ApplicationRole.CanRead);
        public static class ApplicationRole
        {
            public const string CanRead = "DaemonAppRole";
            public const string CanWrite = "DataWriterRole";
        }
        */
        //app.Logger.LogInformation("Routing Request to Service");
        return service.Run(input);
    });
    //todo - implement authorization
    //.RequireAuthorization();

    #endregion configuration

    app.Logger.LogInformation("Awaiting Web Application");
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Program Exception");
}
finally
{
    Log.CloseAndFlush();
}

#region helpers

static void AddCustomHeader(HttpContext context, WebApplication app)
{
    app.Logger.LogInformation("Adding Custom Header");

    //link - https://code-maze.com/aspnetcore-add-custom-headers/
    const string headerKey = "CustomHeader";
    var headerValue = app.Configuration.GetSection(headerKey).Value;

    app.Logger.LogDebug("Logging Custom Header");
    app.Logger.LogDebug("$headerKey = {headerKey} ; $headerValue = {headerValue}", headerKey, headerValue);

    context.Response.Headers.Add(headerKey, headerValue);

}

#endregion helpers
