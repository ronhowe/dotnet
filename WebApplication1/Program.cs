using ClassLibrary1;
using Serilog;
using Serilog.Events;

const string outputTemplate = "[HOST] [{Timestamp:HH:mm:ss.fff zzz}] [CODE TEMPLATE] [{MachineName}] [{Level}] [{SourceContext}]\n    @ {Message}{NewLine}{Exception}";

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .WriteTo.Console(outputTemplate: outputTemplate)
    .CreateLogger();

Log.ForContext("SourceContext", "CUSTOM CONTEXT").Information("Starting Program");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((hostContext, LoggerConfiguration) =>
    {
        LoggerConfiguration.ReadFrom.Configuration(hostContext.Configuration);
    });

    builder.Services.AddScoped<IInterface1, Class1>();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.Logger.LogDebug("Using Developer Exception Page");
        app.UseDeveloperExceptionPage();
    }

    app.Logger.LogDebug("Using Serilog Request Logging");
    app.UseSerilogRequestLogging();

    app.MapGet("/", (bool? input, IInterface1 application) =>
    {
        app.Logger.LogDebug("Getting MockException Setting");
        if (app.Configuration.GetValue<bool>("MockException", false))
        {
            app.Logger.LogWarning("Throwing MockException");
            throw new NotImplementedException("MockException (Suggestion: Set MockException = false in application settings to resolve.)");
        }
        else
        {
            app.Logger.LogDebug("Calling Service");
            return application.Method1(input);
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
