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
    private readonly IConfiguration _config;
    private readonly IFeatureManager _featureManager;

    public Service1HealthCheck(ILogger<Service1> logger, IConfiguration config, IFeatureManager featureManager)
    {
        _logger = logger;
        _config = config;
        _featureManager = featureManager;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Entering {name}", nameof(Service1HealthCheck));

        //help - example of reading boolean from config via iconfiguration
        //_logger.LogDebug("Logging Mock Service Permanent Exception Toggle Value");
        //var config = _config.GetSection(nameof(Service1Feature.MockService1PermanentExceptionToggle)).Value;
        //_logger.LogDebug("config = {config}", config);

        //help - example of reading boolean from config via ifeaturemanager
        _logger.LogDebug("Logging Mock Service Permanent Exception Toggle Value");
        var feature = _featureManager.IsEnabledAsync(nameof(Service1Feature.MockService1PermanentExceptionToggle)).Result;
        _logger.LogDebug("feature = {feature}", feature);

        HealthCheckResult result;

        if (feature)
        {
            //todo - implement robust health check logic
            //todo - inject configuration and logging dependencies

            _logger.LogWarning("Throwing Mock Service Permanent Exception");
            result = new HealthCheckResult(context.Registration.FailureStatus, "UNHEALTHY");
        }
        else
        {
            _logger.LogInformation("Skipping Mock Service Permanent Exception");
            result = HealthCheckResult.Healthy("HEALTHY");
        }

        _logger.LogInformation("Exiting {name}", nameof(Service1HealthCheck));

        return Task.FromResult(result);
    }
}
