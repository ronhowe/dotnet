using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;

namespace FunctionApp1;

public static class Function1
{
    [FunctionName("Function1")]
    public static IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "Service1")] HttpRequest req, ILogger log)
    {
        log.LogInformation("Running Function");

        log.LogWarning("TODO - Inject IConfiguration And IService");
        _ = Boolean.TryParse(req.Query["input"].ToString(), out bool input);
        log.LogDebug($"input={input}");

        return new OkObjectResult(input);
    }
}
