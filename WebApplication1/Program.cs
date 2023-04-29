using ClassLibrary1;

using Serilog;
using Serilog.Events;

const string outputTemplate = "[HOST] [{Timestamp:HH:mm:ss.fff zzz}] [PROGRAM TEMPLATE] [{MachineName}] [{Level}] [{SourceContext}] @ {Message}{NewLine}{Exception}";

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .WriteTo.Console(outputTemplate: outputTemplate)
    .CreateLogger();

Log.Information("Starting Program"); // or Log.ForContext("SourceContext", "TODO: Pick Good Context").Information("Starting");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((hostContext, LoggerConfiguration) =>
    {
        LoggerConfiguration.ReadFrom.Configuration(hostContext.Configuration);
    });

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseSerilogRequestLogging();

    app.MapGet("/", (bool input) =>
    {
        if (app.Configuration.GetValue<bool>("MockException", false))
        {
            app.Logger.LogWarning("MockException = true");
            throw new NotImplementedException("MockException (Suggestion: Set MockException = false in application settings to resolve.)");
        }
        else
        {
            app.Logger.LogInformation("Calling ClassLibrary1.Class1.Method1");
            var class1 = new Class1();
            return class1.Method1(input);
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
