/*******************************************************************************
https://github.com/ronhowe/dotnet
*******************************************************************************/

using ClassLibrary1;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Polly;
using Polly.Contrib.WaitAndRetry;
using System.Diagnostics;
using System.Net;

namespace TestProject1;

[TestClass]
public class UnitTest1
{
    private readonly string _asterisk = new('*', 80);
    private readonly string _enter = new('>', 80);
    private readonly string _exit = new('<', 80);

    [TestInitialize]
    public void TestInitialize()
    {
        Debug.WriteLine(_asterisk);
        Debug.WriteLine("Initializing Test");
        Debug.WriteLine(_asterisk);
    }

    [TestMethod]
    public async Task ClientConnectsToRonHoweNet()
    {
        var retryPolicy = Policy
            .Handle<HttpRequestException>()
            .RetryAsync(5, (exception, retryCount, context) =>
            {
                Debug.WriteLine(_asterisk);
                Debug.WriteLine($"RETRY ATTEMPT # {retryCount}");
                Debug.WriteLine(_asterisk);
            });

        var handler = new HttpClientHandler()
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        using var client = new HttpClient(handler);

        using var response = retryPolicy.ExecuteAsync(async () =>
        {
            Debug.WriteLine(_enter);
            Debug.WriteLine("Starting HTTP Request");
            Debug.WriteLine(_enter);

            return await client.GetAsync("https://ronhowe.net");
        }).Result;

        Debug.WriteLine(_exit);
        Debug.WriteLine("Ending HTTP Request");
        Debug.WriteLine(_exit);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        foreach (var header in response.Headers)
        {
            Trace.WriteLine($"{header.Key} = {header.Value.FirstOrDefault<string>()}");
        }

        Debug.WriteLine(await response.Content.ReadAsStringAsync());
    }

    [TestMethod]
    public void ClientRetriesFromInternalServiceError()
    {
        var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(1), retryCount: 10);

        var retryPolicy = Policy
            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .OrResult(r => r.StatusCode == HttpStatusCode.InternalServerError)
            .WaitAndRetryAsync(delay, (response, timeSpan, retryCount, context) =>
            {
                Debug.WriteLine(_asterisk);
                Debug.WriteLine($"RETRY ATTEMPT # {retryCount} AFTER {timeSpan} SECONDS");
                Debug.WriteLine(_asterisk);
            });

        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, configBuilder) =>
            {
                configBuilder.AddInMemoryCollection(new Dictionary<string, string?> { { "MockService1TransientExceptionToggle", "true" } });
            });
        });

        Debug.WriteLine(_asterisk);
        Debug.WriteLine("Starting Web Application");
        Debug.WriteLine(_asterisk);

        using var client = application.CreateClient();

        using var response = retryPolicy.ExecuteAsync(async () =>
        {
            Debug.WriteLine(_enter);
            Debug.WriteLine("Starting HTTP Request");
            Debug.WriteLine(_enter);

            return await client.GetAsync($"{Service1Endpoint.Service1}?input={Boolean.FalseString}");
        }).Result;

        Debug.WriteLine(_exit);
        Debug.WriteLine("Ending HTTP Request");
        Debug.WriteLine(_exit);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        response.StatusCode.Should<HttpStatusCode>().Be(HttpStatusCode.OK);
    }

    [TestMethod]
    [Ignore]
    public async Task ClientRetriesFromHttpRequestException()
    {
        var retryPolicy = Policy
            .Handle<HttpRequestException>()
            .RetryAsync(5, (exception, retryCount, context) =>
            {
                Debug.WriteLine(_asterisk);
                Debug.WriteLine($"RETRY ATTEMPT # {retryCount}");
                Debug.WriteLine(_asterisk);
            });

        var handler = new HttpClientHandler()
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        using var client = new HttpClient(handler);

        using var response = retryPolicy.ExecuteAsync(async () =>
        {
            Debug.WriteLine(_enter);
            Debug.WriteLine("Starting HTTP Request");
            Debug.WriteLine(_enter);

            return await client.GetAsync("https://localhost:444/health");
        }).Result;

        Debug.WriteLine(_exit);
        Debug.WriteLine("Ending HTTP Request");
        Debug.WriteLine(_exit);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        foreach (var header in response.Headers)
        {
            Trace.WriteLine($"{header.Key} = {header.Value.FirstOrDefault<string>()}");
        }

        Debug.WriteLine(await response.Content.ReadAsStringAsync());
    }
}