using Polly;
using Polly.Retry;
using Shouldly;
using System.Diagnostics;
using System.Net;

namespace MyTestProject10;

[TestClass]
public sealed class MyLiveTests : TestBase
{
    [Ignore]
    [TestMethod]
    [TestCategory("LiveTest")]
    [DataRow("https://localhost:443/healthcheck", "MyHeader (Production)", HttpStatusCode.OK)]
    [DataRow("https://lab-web-00:443/healthcheck", "MyHeader (Production)", HttpStatusCode.OK)]
    [DataRow("https://app-ronhowe-0.azurewebsites.net:443/healthcheck", "MyHeader (Production)", HttpStatusCode.OK)]
    public void LiveSiteTests(string uriString, string headerValue, HttpStatusCode httpStatusCode)
    {
        // TODO: Read retry settings from configuration.
        const int _maxRetries = 1;
        const int _retryMilliseconds = 1;

        Debug.WriteLine("Creating Retry Policy");
        AsyncRetryPolicy retryPolicy = Policy
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(_maxRetries, retryAttempt => TimeSpan.FromMilliseconds(_retryMilliseconds),
                (ex, timeSpan, retryAttempt, context) =>
                {
                    Debug.WriteLine($"HTTP Request Failed Because {ex.Message}");
                    Debug.WriteLine($"Retry Attempt # {retryAttempt} Of {_maxRetries}");
                });

        HttpClientHandler handler = new()
        {
            // NOTE: Needed for localhost and lab environments with untrusted, self-signed certificates.
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        using HttpClient client = new(handler);

        using HttpResponseMessage response = retryPolicy.ExecuteAsync(async () =>
        {
            Debug.WriteLine($"Sending HTTP GET Request");
            return await client.GetAsync(new Uri(uriString), TestContext.CancellationToken);
        }).Result;

        Debug.WriteLine($"Asserting HTTP Status Code Is {httpStatusCode}");
        response.StatusCode.ShouldBe(httpStatusCode);

        Debug.WriteLine($"Logging Headers");
        foreach (var header in response.Headers)
        {
            Debug.WriteLine($"{header.Key} = {header.Value.FirstOrDefault<string>()}");
        }

        Debug.WriteLine($"Asserting MyHeader Contains {headerValue}");
        response.Headers.Any(h => h.Key == "MyHeader" && h.Value.Contains(headerValue)).ShouldBeTrue();

        Debug.WriteLine($"Asserting Health Check Is Healthy");
        (response.Content.ReadAsStringAsync(TestContext.CancellationToken).Result).ShouldBe("Healthy");
    }

    public TestContext TestContext { get; set; }
}
