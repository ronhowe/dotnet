/*******************************************************************************
https://github.com/ronhowe/dotnet
*******************************************************************************/

using ClassLibrary1;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;

namespace FunctionApp1;

public class Function1
{
    private readonly ILogger _logger;
    private readonly IConfiguration _configuration;
    private readonly IService1 _service;
    private readonly IDateTimeService _dateTime;

    public Function1(ILoggerFactory loggerFactory, IConfiguration configuration, IService1 service, IDateTimeService dateTime)
    {
        _logger = loggerFactory.CreateLogger<Function1>();
        _configuration = configuration;
        _service = service;
        _dateTime = dateTime;
    }

    [Function("Function1")]
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "service1")] HttpRequestData req)
    {
        _logger.LogInformation("Entering {name}", nameof(Function1));

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

        _logger.LogDebug("Adding Custom Header");

        //link - https://code-maze.com/aspnetcore-add-custom-headers/
        const string headerKey = "CustomHeader";
        var headerValue = _configuration.GetSection(headerKey).Value;

        response.Headers.Add(headerKey, headerValue);

        _logger.LogDebug("Logging Custom Header");
        _logger.LogDebug("$headerKey = {headerKey} ; $headerValue = {headerValue}", headerKey, headerValue);

        _service.Run(Boolean.TryParse(req.Query["input"], out bool output));

        response.WriteString(output.ToString());

        _logger.LogInformation("Exiting {name}", nameof(Function1));

        return response;
    }
}
