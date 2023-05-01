using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;

namespace ClassLibrary1;

public class Service : IService
{
    private readonly ILogger<Service> _logger;
    private readonly IConfiguration _config;
    private readonly IFeatureManager _featureManager;

    public Service(ILogger<Service> logger, IConfiguration config, IFeatureManager featureManager)
    {
        _logger = logger;
        _config = config;
        _featureManager = featureManager;
    }

    public bool Run(bool? input)
    {
        _logger.LogInformation("{input}", input);

        var config = _config.GetSection(nameof(FeatureFlags.MockServiceExceptionEnabled)).Value;
        var feature = _featureManager.IsEnabledAsync(nameof(FeatureFlags.MockServiceExceptionEnabled)).Result;

        if (config != null && Boolean.Parse(config) && feature)
        {
            throw new MockServiceException(nameof(FeatureFlags.MockServiceExceptionEnabled));
        }

        return input != null && input.Value;
    }
}