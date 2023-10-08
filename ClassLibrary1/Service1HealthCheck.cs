/*******************************************************************************
https://github.com/ronhowe/dotnet
*******************************************************************************/

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;

namespace ClassLibrary1;

public class Service1HealthCheck : IHealthCheck
{
    private readonly ILogger<Service1> _logger;
    private readonly IConfiguration _configuration;
    private readonly IFeatureManager _featureManager;
    private readonly IDateTimeService _dateTime;

    public Service1HealthCheck(ILogger<Service1> logger, IConfiguration configuration, IFeatureManager featureManager, IDateTimeService dateTime)
    {
        _logger = logger;
        _configuration = configuration;
        _featureManager = featureManager;
        _dateTime = dateTime;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Entering {name}", nameof(Service1HealthCheck));

        _logger.LogDebug("Logging Mock Service Permanent Exception Toggle Value");
        var feature = _featureManager.IsEnabledAsync(nameof(Service1Feature.MockService1PermanentExceptionToggle)).Result;
        _logger.LogDebug("feature = {feature}", feature);

        HealthCheckResult result = HealthCheckResult.Healthy("HEALTHY");

        if (feature)
        {
            _logger.LogWarning("Throwing Mock Service Permanent Exception");
            result = new HealthCheckResult(context.Registration.FailureStatus, "UNHEALTHY");
        }
        else
        {
            _logger.LogInformation("Skipping Mock Service Permanent Exception");

            _logger.LogDebug("Logging Mock Service Transient Exception Toggle Value");
            feature = _featureManager.IsEnabledAsync(nameof(Service1Feature.MockService1TransientExceptionToggle)).Result;
            _logger.LogDebug("$feature = {feature}", feature);

            if (feature)
            {
                _logger.LogInformation("Considering Throwing Mock Service Transient Exception");

                if (_dateTime.UtcNow.Ticks % 2 != 0) // odd ticks
                {
                    _logger.LogWarning("Throwing Mock Service Transient Exception");
                    result = new HealthCheckResult(context.Registration.FailureStatus, "UNHEALTHY");
                }
                else
                {
                    _logger.LogInformation("Avoiding Mock Service Transient Exception");
                }
            }
            else
            {
                _logger.LogInformation("Skipping Mock Service Transient Exception");
            }
        }

        _logger.LogDebug("Logging Mock Service CPU Throttle Toggle Value");
        feature = _featureManager.IsEnabledAsync(nameof(Service1Feature.MockService1CpuThrottleToggle)).Result;
        _logger.LogDebug("$feature = {feature}", feature);

        if (feature)
        {
            _logger.LogWarning("Throttling CPU");

            int iterations = _configuration.GetValue<int>(nameof(Service1Feature.MockService1CpuThrottleIterations), 1);

            _logger.LogDebug("Logging Mock Service CPU Throttle Iterations Value");
            _logger.LogDebug("$iterations = {iterations}", iterations);

            if (iterations > 20000)
            {
                iterations = 20000;
            }

            List<int> primes = new();

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
                    _logger.LogDebug("$i = {i}", i);
                    primes.Add(i);
                }

                isPrime = true;
            }

            _logger.LogWarning("Dethrottling CPU");
        }
        else
        {
            _logger.LogInformation("Skipping Mock Service CPU Throttle");
        }

        _logger.LogInformation("Exiting {name}", nameof(Service1HealthCheck));

        return Task.FromResult(result);
    }
}
