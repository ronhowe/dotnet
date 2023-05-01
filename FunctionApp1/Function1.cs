using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace FunctionApp1;

public static class Function1
{
    [FunctionName("Function1")]
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req, ILogger log)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        log.LogInformation("Running");

        _ = Boolean.TryParse(req.Query["input"].ToString(), out bool input);

        log.LogDebug($"input={input}");

        return new OkObjectResult(input);
    }
}
