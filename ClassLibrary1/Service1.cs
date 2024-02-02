/*******************************************************************************
https://github.com/ronhowe
*******************************************************************************/

using Microsoft.Extensions.Logging;

namespace ClassLibrary1;

public class Service1(ILogger<Service1> logger) : IService1
{
    public bool Run(bool input)
    {
        logger.LogInformation("Entering {name}", nameof(Service1));

        logger.LogDebug("Logging Input Parameter(s) and Value(s)");
        logger.LogDebug("$input = {input}", input);

        // very important business logic
        bool result = input;

        logger.LogInformation("Exiting {name}", nameof(Service1));

        return result;
    }
}
