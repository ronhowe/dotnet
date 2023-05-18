using ClassLibrary1;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;

namespace FunctionApp1
{
    public class Function1
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IService1 _service;

        public Function1(ILoggerFactory loggerFactory, IConfiguration configuration, IService1 service)
        {
            _logger = loggerFactory.CreateLogger<Function1>();
            _configuration = configuration;
            _service = service;
        }

        [Function("Function1")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "service1")] HttpRequestData req)
        {
            _logger.LogInformation("Running Function");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            //link - https://code-maze.com/aspnetcore-add-custom-headers/
            const string headerKey = "CustomHeader";
            var headerValue = _configuration.GetSection(headerKey).Value;
            _logger.LogDebug("Adding Custom Header {headerKey}={headerValue}", headerKey, headerValue);
            response.Headers.Add(headerKey, headerValue);

            _service.Run(Boolean.TryParse(req.Query["input"], out bool output));

            response.WriteString(output.ToString());

            return response;
        }
    }
}
