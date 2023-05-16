using ClassLibrary1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;

namespace FunctionApp1;

public class Function1
{
#pragma warning disable IDE0052 // Remove unread private members
    private readonly HttpClient _client;
#pragma warning restore IDE0052 // Remove unread private members
    private readonly IService1 _service;

    public Function1(IHttpClientFactory httpClientFactory, IService1 service)
    {
        //help - https://www.youtube.com/watch?v=ffnJTvJujaM
        _client = httpClientFactory.CreateClient();
        _service = service;
    }

    [FunctionName("Function1")]
    public IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = ApplicationEndpoint.Service1)] HttpRequest req,
        ILogger log
    )
    {
        log.LogInformation("Running Function");

        _service.Run(Boolean.TryParse(req.Query["input"], out bool output));

        return new OkObjectResult(output);
    }
}
