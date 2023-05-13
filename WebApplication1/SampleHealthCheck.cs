using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace WebApplication1;

public class SampleHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var isHealthy = true;

        // TODO - Implement Robust Health Check Logic
        // TODO - Inject Configuration And Logging Dependencies

        if (isHealthy)
        {
            return Task.FromResult(
                HealthCheckResult.Healthy("HEALTHY"));
        }

        return Task.FromResult(
            new HealthCheckResult(
                context.Registration.FailureStatus, "UNHEALTHY"));
    }
}
