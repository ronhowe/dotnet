/*******************************************************************************
https://github.com/ronhowe
*******************************************************************************/

namespace WebApplication1;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        _logger.LogInformation("================> Incoming HTTP Request");
        await _next(context);
        _logger.LogInformation("<================ Outgoing HTTP Response");
    }
}
