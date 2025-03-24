using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace MyClassLibrary;

public class MyHealthCheck(ILogger<MyService> logger, IMyService myService) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Entering {name}", nameof(MyHealthCheck));

        HealthCheckResult _result;

        try
        {
            logger.LogInformation("Calling MyService.MyMethod(false)");
            await myService.MyMethodAsync(false);
            _result = HealthCheckResult.Healthy("HEALTHY");
        }
        catch (Exception ex)
        {
            logger.LogError("Error Calling MyService.MyMethod(false) Because {message}", ex.Message);
            _result = new HealthCheckResult(context.Registration.FailureStatus, "UNHEALTHY");
        }

        logger.LogDebug("_result = {_result}", _result);

        logger.LogInformation("Exiting {name}", nameof(MyHealthCheck));

        return await Task.FromResult(_result);
    }
}
