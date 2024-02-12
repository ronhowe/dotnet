/*******************************************************************************
https://github.com/ronhowe
*******************************************************************************/

namespace WebApplication1;

public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        logger.LogInformation("Incoming HTTP Request");
        await next(context);
        logger.LogInformation("Outgoing HTTP Response");
    }
}
