using ClassLibrary1;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(FunctionApp1.Startup))]
namespace FunctionApp1;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddHttpClient();
        builder.Services.AddScoped<IService1, Service1>();
        //builder.Services.AddSingleton<IService1>((s) =>
        //{
        //    return new Service1();
        //});
    }
}
