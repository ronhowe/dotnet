/*******************************************************************************
https://github.com/ronhowe
*******************************************************************************/

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;

namespace ClassLibrary1;

public class Service1HealthCheck(ILogger<Service1> logger, IConfiguration configuration, IFeatureManager featureManager, IDateTimeService dateTime) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Entering {name}", nameof(Service1HealthCheck));

        HealthCheckResult result = HealthCheckResult.Healthy("HEALTHY");

        logger.LogDebug("Logging Mock Service Permanent Exception Toggle Value");
        var feature = featureManager.IsEnabledAsync(nameof(Service1Feature.MockService1PermanentExceptionToggle)).Result;
        logger.LogDebug("feature = {feature}", feature);

        if (feature)
        {
            logger.LogWarning("Throwing Mock Service Permanent Exception");
            return Task.FromResult(new HealthCheckResult(context.Registration.FailureStatus, "UNHEALTHY"));
        }
        else
        {
            logger.LogInformation("Skipping Mock Service Permanent Exception");
        }

        logger.LogDebug("Logging Mock Service Transient Exception Toggle Value");
        feature = featureManager.IsEnabledAsync(nameof(Service1Feature.MockService1TransientExceptionToggle)).Result;
        logger.LogDebug("$feature = {feature}", feature);

        if (feature && (dateTime.UtcNow.Ticks % 2 != 0)) // odd ticks
        {
            logger.LogWarning("Throwing Mock Service Transient Exception");
            return Task.FromResult(new HealthCheckResult(context.Registration.FailureStatus, "UNHEALTHY"));
        }
        else
        {
            logger.LogInformation("Skipping Mock Service Transient Exception");
        }

        logger.LogDebug("Logging Mock Service CPU Throttle Toggle Value");
        feature = featureManager.IsEnabledAsync(nameof(Service1Feature.MockService1CpuThrottleToggle)).Result;
        logger.LogDebug("$feature = {feature}", feature);

        if (feature)
        {
            logger.LogWarning("Throttling CPU");

            int iterations = configuration.GetValue<int>(nameof(Service1Feature.MockService1CpuThrottleIterations), 1);

            logger.LogDebug("Logging Mock Service CPU Throttle Iterations Value");
            logger.LogDebug("$iterations = {iterations}", iterations);

            ThrottleCpuWithPrimeNumberMath(iterations);

            logger.LogWarning("Dethrottling CPU");
        }
        else
        {
            logger.LogInformation("Skipping Mock Service CPU Throttle");
        }

        logger.LogInformation("Exiting {name}", nameof(Service1HealthCheck));

        return Task.FromResult(result);

        static void ThrottleCpuWithPrimeNumberMath(int iterations)
        {
            List<int> primes = [];

            bool isPrime = true;

            for (int i = 2; i <= iterations; i++)
            {
                for (int j = 2; j <= iterations; j++)
                {
                    if (i != j && i % j == 0)
                    {
                        isPrime = false;
                        break;
                    }
                }

                if (isPrime)
                {
                    primes.Add(i);
                }

                isPrime = true;
            }
        }
    }
}
