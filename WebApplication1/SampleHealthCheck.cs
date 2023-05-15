using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace WebApplication1;

public class SampleHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var isHealthy = true;

        // todo - implement robust health check logic
        // todo - inject configuration and logging dependencies

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
