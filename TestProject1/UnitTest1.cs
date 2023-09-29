/*******************************************************************************
https://github.com/ronhowe/dotnet
*******************************************************************************/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Polly;
using Polly.Contrib.WaitAndRetry;
using System.Diagnostics;
using System.Net;

namespace TestProject1;

[TestClass]
public class UnitTest1
{
    [TestInitialize]
    public void TestInitialize()
    {
        Debug.WriteLine("Initializing Test");
    }

    [TestMethod]
    [Ignore]
    public async Task TestMethod1()
    {
        var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(1), retryCount: 5);

        //var retryPolicy = Policy
        //    .Handle<HttpRequestException>()
        //    .RetryAsync(5, (exception, retryCount, context) =>
        //    {
        //        Log.ForContext("SourceContext", sourceContext).Warning($"Retry attempt {retryCount} fired!");
        //    });

        var retryPolicy = Policy
            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode || r.StatusCode == HttpStatusCode.InternalServerError)
            .WaitAndRetryAsync(delay, (response, timeSpan, retryCount, context) =>
            {
                Debug.WriteLine($"RETRY ATTEMPT # {retryCount} AFTER {timeSpan} SECONDS");
            });

        var handler = new HttpClientHandler()
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        using var client = new HttpClient(handler);

        using var response = retryPolicy.ExecuteAsync(async () =>
        {
            return await client.GetAsync("https://localhost:444/health");
        }).Result;

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        foreach (var header in response.Headers)
        {
            Trace.WriteLine($"{header.Key} = {header.Value.FirstOrDefault<string>()}");
        }

        Debug.WriteLine(await response.Content.ReadAsStringAsync());
    }
}