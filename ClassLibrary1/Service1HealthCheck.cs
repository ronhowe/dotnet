using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;

namespace ClassLibrary1;

public class Service1HealthCheck : IHealthCheck
{
    private readonly ILogger<Service1> _logger;
    private readonly IConfiguration _config;
    private readonly IFeatureManager _featureManager;

    public Service1HealthCheck(ILogger<Service1> logger, IConfiguration config, IFeatureManager featureManager)
    {
        _logger = logger;
        _config = config;
        _featureManager = featureManager;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Running Health Check");

        var config = _config.GetSection(nameof(Service1Feature.MockService1ExceptionToggle)).Value;
        _logger.LogDebug("config={config}", config);

        //help - example of reading boolean from config via ifeaturemanager
        var feature = _featureManager.IsEnabledAsync(nameof(Service1Feature.MockService1ExceptionToggle)).Result;
        _logger.LogDebug("feature={feature}", feature);

        if (feature)
        {
            _logger.LogWarning("Throwing MockServiceException");

            //todo - implement robust health check logic
            //todo - inject configuration and logging dependencies

            return Task.FromResult(
            new HealthCheckResult(
                context.Registration.FailureStatus, "UNHEALTHY"));
        }

        return Task.FromResult(
            HealthCheckResult.Healthy("HEALTHY"));
    }
}
