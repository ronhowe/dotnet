/*******************************************************************************
https://github.com/ronhowe
*******************************************************************************/

using ClassLibrary1;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;

namespace FunctionApp1;

public class Function1(ILoggerFactory loggerFactory, IConfiguration configuration, IService1 service)
{
    private readonly ILogger logger = loggerFactory.CreateLogger<Function1>();

    [Function("Function1")]
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "service1")] HttpRequestData req)
    {
        logger.LogInformation("Entering {name}", nameof(Function1));

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

        logger.LogDebug("Adding Custom Header");

        //link - https://code-maze.com/aspnetcore-add-custom-headers/
        const string headerKey = "CustomHeader";
        var headerValue = configuration.GetSection(headerKey).Value;

        response.Headers.Add(headerKey, headerValue);

        logger.LogDebug("Logging Custom Header");
        logger.LogDebug("$headerKey = {headerKey} ; $headerValue = {headerValue}", headerKey, headerValue);

        service.Run(Boolean.TryParse(req.Query["input"], out bool output));

        response.WriteString(output.ToString());

        logger.LogInformation("Exiting {name}", nameof(Function1));

        return response;
    }
}
