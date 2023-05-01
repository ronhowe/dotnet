using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ClassLibrary1;

public class Service : IService
{
    private const string configKey = "MockExceptionEnabled";

    private readonly ILogger<Service> _logger;
    private readonly IConfiguration _config;

    public Service(ILogger<Service> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;
    }

    public bool Run(bool? input)
    {
        _logger.LogInformation("{input}", input);

        var config = _config.GetSection(configKey).Value;

        if (config != null && Boolean.Parse(config))
        {
            throw new NotImplementedException(configKey);
        }

        return input != null && input.Value;
    }
}