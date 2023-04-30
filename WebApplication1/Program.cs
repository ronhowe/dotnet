using ClassLibrary1;
using Serilog;
using Serilog.Events;

const string outputTemplate = "[HOST] [{Timestamp:HH:mm:ss.fff zzz}] [CODE TEMPLATE] [{MachineName}] [{Level}] [{SourceContext}]\n    @ {Message}{NewLine}{Exception}";

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .WriteTo.Console(outputTemplate: outputTemplate)
    .CreateLogger();

Log.ForContext("SourceContext", "CUSTOM CONTEXT").Debug("Starting Program");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((hostContext, LoggerConfiguration) =>
    {
        LoggerConfiguration.ReadFrom.Configuration(hostContext.Configuration);
    });

    builder.Services.AddScoped<IService, Service>();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.Logger.LogDebug("Using Developer Exception Page");
        app.UseDeveloperExceptionPage();
    }

    app.Logger.LogDebug("Using Serilog Request Logging");
    app.UseSerilogRequestLogging();

    app.MapGet("/", (bool? input, IService service) =>
    {
        app.Logger.LogDebug("Getting Configuration");
        if (app.Configuration.GetValue<bool>("MockException", false))
        {
            app.Logger.LogWarning("Throwing MockException");
            throw new NotImplementedException("MockException");
        }
        else
        {
            app.Logger.LogDebug("Calling Service");
            return service.Run(input);
        }
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
