using Microsoft.Extensions.Logging;

namespace ClassLibrary1;

public class Service : IService
{
    private readonly ILogger<Service> _logger;

    public Service(ILogger<Service> logger)
    {
        _logger = logger;
    }

    public bool Run(bool? input)
    {
        bool output = input != null && input.Value;
        _logger.LogDebug("{output}", output);
        return output;
    }
}