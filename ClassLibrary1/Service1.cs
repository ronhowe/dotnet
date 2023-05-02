using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;

namespace ClassLibrary1;

public class Service1 : IService1
{
    private readonly ILogger<Service1> _logger;
    private readonly IConfiguration _config;
    private readonly IFeatureManager _featureManager;

    public Service1(ILogger<Service1> logger, IConfiguration config, IFeatureManager featureManager)
    {
        _logger = logger;
        _config = config;
        _featureManager = featureManager;
    }

    public bool Run(bool? input)
    {
        _logger.LogInformation("Running");

        _logger.LogDebug("input={input}", input);

        var config = _config.GetSection(nameof(ServiceFeatures.MockServiceExceptionToggle)).Value;
        _logger.LogDebug("config={config}", config);

        var feature = _featureManager.IsEnabledAsync(nameof(ServiceFeatures.MockServiceExceptionToggle)).Result;
        _logger.LogDebug("feature={feature}", feature);

        if (feature)
        {
            throw new MockServiceException(nameof(ServiceFeatures.MockServiceExceptionToggle));
        }

        return input != null && input.Value;
    }
}