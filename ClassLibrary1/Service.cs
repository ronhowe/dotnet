using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ClassLibrary1;

public class Service : IService
{
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

        var config = _config.GetSection(MockException.KeyName).Value;

        if (config != null && Boolean.Parse(config))
        {
            throw new MockException(MockException.KeyName);
        }

        return input != null && input.Value;
    }
}