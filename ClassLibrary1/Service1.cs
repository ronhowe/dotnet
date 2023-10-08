/*******************************************************************************
https://github.com/ronhowe/dotnet
*******************************************************************************/

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;

namespace ClassLibrary1;

public class Service1 : IService1
{
    private readonly ILogger<Service1> _logger;
    private readonly IConfiguration _configuration;
    private readonly IFeatureManager _featureManager;
    private readonly IDateTimeService _dateTime;

    public Service1(ILogger<Service1> logger, IConfiguration configuration, IFeatureManager featureManager, IDateTimeService dateTime)
    {
        _logger = logger;
        _configuration = configuration;
        _featureManager = featureManager;
        _dateTime = dateTime;
    }

    public bool Run(bool input)
    {
        _logger.LogInformation("Entering {name}", nameof(Service1));

        _logger.LogDebug("Logging Input Parameter(s) and Value(s)");
        _logger.LogDebug("$input = {input}", input);

        // very important business logic
        bool result = input;

        _logger.LogInformation("Exiting {name}", nameof(Service1));

        return result;
    }
}
