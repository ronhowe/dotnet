using Microsoft.Extensions.Logging;

namespace ClassLibrary1;

public class Service : IService
{
    private readonly ILogger<Service> _logger;

    public Service(ILogger<Service> logger)
    {
        _logger = logger;
    }

    public bool IO(bool? input)
    {
        _logger.LogDebug("IO");
        return input != null && input.Value;
    }
}