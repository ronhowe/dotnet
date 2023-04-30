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

Log.ForContext("SourceContext", "Program").Information("OK");

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
        app.UseDeveloperExceptionPage();
    }

    app.UseSerilogRequestLogging();

    app.MapGet("/", (bool? input, IService service) =>
    {
        if (app.Configuration.GetValue<bool>("MockRouteExceptionEnabled", false))
        {
            throw new NotImplementedException("MockRouteExceptionEnabled");
        }
        else
        {
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
