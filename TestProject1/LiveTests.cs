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
using Serilog;
using System.Net;

namespace TestProject1;

//[TestClass]
public class LiveTests : TestBase
{
    //[TestMethod]
    public async Task ClientConnectsToRonHoweNet()
    {
        var retryPolicy = Policy
            .Handle<HttpRequestException>()
            .RetryAsync(5, (exception, retryCount, context) =>
            {
                Log.ForContext("SourceContext", _sourceContext).Debug(_asterisk);
                Log.ForContext("SourceContext", _sourceContext).Debug($"RETRY ATTEMPT # {retryCount}");
                Log.ForContext("SourceContext", _sourceContext).Debug(_asterisk);
            });

        var handler = new HttpClientHandler()
        {
            //ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        using var client = new HttpClient(handler);

        using var response = retryPolicy.ExecuteAsync(async () =>
        {
            Log.ForContext("SourceContext", _sourceContext).Debug(_enter);
            Log.ForContext("SourceContext", _sourceContext).Debug("Starting HTTP Request");
            Log.ForContext("SourceContext", _sourceContext).Debug(_enter);

            return await client.GetAsync("https://ronhowe.net");
        }).Result;

        Log.ForContext("SourceContext", _sourceContext).Debug(_exit);
        Log.ForContext("SourceContext", _sourceContext).Debug("Ending HTTP Request");
        Log.ForContext("SourceContext", _sourceContext).Debug(_exit);

        Assert.AreEqual<HttpStatusCode>(HttpStatusCode.OK, response.StatusCode);
        response.StatusCode.Should<HttpStatusCode>().Be(HttpStatusCode.OK);

        Log.ForContext("SourceContext", _sourceContext).Debug(_exit);
        Log.ForContext("SourceContext", _sourceContext).Debug("Logging Headers");
        Log.ForContext("SourceContext", _sourceContext).Debug(_exit);

        foreach (var header in response.Headers)
        {
            Log.ForContext("SourceContext", _sourceContext).Debug($"{header.Key} = {header.Value.FirstOrDefault<string>()}");
        }

        Log.ForContext("SourceContext", _sourceContext).Debug(_exit);
        Log.ForContext("SourceContext", _sourceContext).Debug("Logging Content");
        Log.ForContext("SourceContext", _sourceContext).Debug(_exit);

        Log.ForContext("SourceContext", _sourceContext).Debug(await response.Content.ReadAsStringAsync());
    }

    //[TestMethod]
    public async Task ClientConnectsToFrontDoor()
    {
        var retryPolicy = Policy
            .Handle<HttpRequestException>()
            .RetryAsync(5, (exception, retryCount, context) =>
            {
                Log.ForContext("SourceContext", _sourceContext).Debug(_asterisk);
                Log.ForContext("SourceContext", _sourceContext).Debug($"RETRY ATTEMPT # {retryCount}");
                Log.ForContext("SourceContext", _sourceContext).Debug(_asterisk);
            });

        var handler = new HttpClientHandler()
        {
            //ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        using var client = new HttpClient(handler);

        using var response = retryPolicy.ExecuteAsync(async () =>
        {
            Log.ForContext("SourceContext", _sourceContext).Debug(_enter);
            Log.ForContext("SourceContext", _sourceContext).Debug("Starting HTTP Request");
            Log.ForContext("SourceContext", _sourceContext).Debug(_enter);

            return await client.GetAsync("https://rhowe-fwbuh9b9cxbdhrgs.z01.azurefd.net/health");
        }).Result;

        Log.ForContext("SourceContext", _sourceContext).Debug(_exit);
        Log.ForContext("SourceContext", _sourceContext).Debug("Ending HTTP Request");
        Log.ForContext("SourceContext", _sourceContext).Debug(_exit);

        Assert.AreEqual<HttpStatusCode>(HttpStatusCode.OK, response.StatusCode);
        response.StatusCode.Should<HttpStatusCode>().Be(HttpStatusCode.OK);

        Log.ForContext("SourceContext", _sourceContext).Debug(_exit);
        Log.ForContext("SourceContext", _sourceContext).Debug("Logging Headers");
        Log.ForContext("SourceContext", _sourceContext).Debug(_exit);

        foreach (var header in response.Headers)
        {
            Log.ForContext("SourceContext", _sourceContext).Debug($"{header.Key} = {header.Value.FirstOrDefault<string>()}");
        }

        Log.ForContext("SourceContext", _sourceContext).Debug(_exit);
        Log.ForContext("SourceContext", _sourceContext).Debug("Logging Content");
        Log.ForContext("SourceContext", _sourceContext).Debug(_exit);

        Log.ForContext("SourceContext", _sourceContext).Debug(await response.Content.ReadAsStringAsync());
    }

    //[TestMethod]
    public async Task ClientConnectsToAzureAppService000()
    {
        var retryPolicy = Policy
            .Handle<HttpRequestException>()
            .RetryAsync(5, (exception, retryCount, context) =>
            {
                Log.ForContext("SourceContext", _sourceContext).Debug(_asterisk);
                Log.ForContext("SourceContext", _sourceContext).Debug($"RETRY ATTEMPT # {retryCount}");
                Log.ForContext("SourceContext", _sourceContext).Debug(_asterisk);
            });

        var handler = new HttpClientHandler()
        {
            //ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        using var client = new HttpClient(handler);

        using var response = retryPolicy.ExecuteAsync(async () =>
        {
            Log.ForContext("SourceContext", _sourceContext).Debug(_enter);
            Log.ForContext("SourceContext", _sourceContext).Debug("Starting HTTP Request");
            Log.ForContext("SourceContext", _sourceContext).Debug(_enter);

            return await client.GetAsync("https://app-rhowe-000.azurewebsites.net/health");
        }).Result;

        Log.ForContext("SourceContext", _sourceContext).Debug(_exit);
        Log.ForContext("SourceContext", _sourceContext).Debug("Ending HTTP Request");
        Log.ForContext("SourceContext", _sourceContext).Debug(_exit);

        Assert.AreEqual<HttpStatusCode>(HttpStatusCode.OK, response.StatusCode);
        response.StatusCode.Should<HttpStatusCode>().Be(HttpStatusCode.OK);

        Log.ForContext("SourceContext", _sourceContext).Debug(_exit);
        Log.ForContext("SourceContext", _sourceContext).Debug("Logging Headers");
        Log.ForContext("SourceContext", _sourceContext).Debug(_exit);

        foreach (var header in response.Headers)
        {
            Log.ForContext("SourceContext", _sourceContext).Debug($"{header.Key} = {header.Value.FirstOrDefault<string>()}");
        }

        Log.ForContext("SourceContext", _sourceContext).Debug(_exit);
        Log.ForContext("SourceContext", _sourceContext).Debug("Logging Content");
        Log.ForContext("SourceContext", _sourceContext).Debug(_exit);

        Log.ForContext("SourceContext", _sourceContext).Debug(await response.Content.ReadAsStringAsync());
    }

    //[TestMethod]
    public async Task ClientConnectsToAzureAppService001()
    {
        var retryPolicy = Policy
            .Handle<HttpRequestException>()
            .RetryAsync(5, (exception, retryCount, context) =>
            {
                Log.ForContext("SourceContext", _sourceContext).Debug(_asterisk);
                Log.ForContext("SourceContext", _sourceContext).Debug($"RETRY ATTEMPT # {retryCount}");
                Log.ForContext("SourceContext", _sourceContext).Debug(_asterisk);
            });

        var handler = new HttpClientHandler()
        {
            //ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        using var client = new HttpClient(handler);

        using var response = retryPolicy.ExecuteAsync(async () =>
        {
            Log.ForContext("SourceContext", _sourceContext).Debug(_enter);
            Log.ForContext("SourceContext", _sourceContext).Debug("Starting HTTP Request");
            Log.ForContext("SourceContext", _sourceContext).Debug(_enter);

            return await client.GetAsync("https://app-rhowe-001.azurewebsites.net/health");
        }).Result;

        Log.ForContext("SourceContext", _sourceContext).Debug(_exit);
        Log.ForContext("SourceContext", _sourceContext).Debug("Ending HTTP Request");
        Log.ForContext("SourceContext", _sourceContext).Debug(_exit);

        Assert.AreEqual<HttpStatusCode>(HttpStatusCode.OK, response.StatusCode);
        response.StatusCode.Should<HttpStatusCode>().Be(HttpStatusCode.OK);

        Log.ForContext("SourceContext", _sourceContext).Debug(_exit);
        Log.ForContext("SourceContext", _sourceContext).Debug("Logging Headers");
        Log.ForContext("SourceContext", _sourceContext).Debug(_exit);

        foreach (var header in response.Headers)
        {
            Log.ForContext("SourceContext", _sourceContext).Debug($"{header.Key} = {header.Value.FirstOrDefault<string>()}");
        }

        Log.ForContext("SourceContext", _sourceContext).Debug(_exit);
        Log.ForContext("SourceContext", _sourceContext).Debug("Logging Content");
        Log.ForContext("SourceContext", _sourceContext).Debug(_exit);

        Log.ForContext("SourceContext", _sourceContext).Debug(await response.Content.ReadAsStringAsync());
    }

    //[TestMethod]
    //[Ignore]
    public void ClientRetriesFromInternalServiceError()
    {
        var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(1), retryCount: 10);

        var retryPolicy = Policy
            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .OrResult(r => r.StatusCode == HttpStatusCode.InternalServerError)
            .WaitAndRetryAsync(delay, (response, timeSpan, retryCount, context) =>
            {
                Log.ForContext("SourceContext", _sourceContext).Debug(_asterisk);
                Log.ForContext("SourceContext", _sourceContext).Debug($"RETRY ATTEMPT # {retryCount} AFTER {timeSpan} SECONDS");
                Log.ForContext("SourceContext", _sourceContext).Debug(_asterisk);
            });

        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, configBuilder) =>
            {
                configBuilder.AddInMemoryCollection(new Dictionary<string, string?> { { "MockService1TransientExceptionToggle", "true" } });
            });
        });

        Log.ForContext("SourceContext", _sourceContext).Debug(_asterisk);
        Log.ForContext("SourceContext", _sourceContext).Debug("Starting Web Application");
        Log.ForContext("SourceContext", _sourceContext).Debug(_asterisk);

        using var client = application.CreateClient();

        using var response = retryPolicy.ExecuteAsync(async () =>
        {
            Log.ForContext("SourceContext", _sourceContext).Debug(_enter);
            Log.ForContext("SourceContext", _sourceContext).Debug("Starting HTTP Request");
            Log.ForContext("SourceContext", _sourceContext).Debug(_enter);

            return await client.GetAsync($"{Service1Endpoint.Service1}?input={Boolean.FalseString}");
        }).Result;

        Log.ForContext("SourceContext", _sourceContext).Debug(_exit);
        Log.ForContext("SourceContext", _sourceContext).Debug("Ending HTTP Request");
        Log.ForContext("SourceContext", _sourceContext).Debug(_exit);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        response.StatusCode.Should<HttpStatusCode>().Be(HttpStatusCode.OK);
    }

    //[TestMethod]
    //[Ignore]
    public async Task ClientRetriesFromHttpRequestException()
    {
        var retryPolicy = Policy
            .Handle<HttpRequestException>()
            .RetryAsync(5, (exception, retryCount, context) =>
            {
                Log.ForContext("SourceContext", _sourceContext).Debug(_asterisk);
                Log.ForContext("SourceContext", _sourceContext).Debug($"RETRY ATTEMPT # {retryCount}");
                Log.ForContext("SourceContext", _sourceContext).Debug(_asterisk);
            });

        var handler = new HttpClientHandler()
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        using var client = new HttpClient(handler);

        using var response = retryPolicy.ExecuteAsync(async () =>
        {
            Log.ForContext("SourceContext", _sourceContext).Debug(_enter);
            Log.ForContext("SourceContext", _sourceContext).Debug("Starting HTTP Request");
            Log.ForContext("SourceContext", _sourceContext).Debug(_enter);

            return await client.GetAsync("https://localhost:444/health");
        }).Result;

        Log.ForContext("SourceContext", _sourceContext).Debug(_exit);
        Log.ForContext("SourceContext", _sourceContext).Debug("Ending HTTP Request");
        Log.ForContext("SourceContext", _sourceContext).Debug(_exit);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        Log.ForContext("SourceContext", _sourceContext).Debug(_exit);
        Log.ForContext("SourceContext", _sourceContext).Debug("Logging Headers");
        Log.ForContext("SourceContext", _sourceContext).Debug(_exit);

        foreach (var header in response.Headers)
        {
            Log.ForContext("SourceContext", _sourceContext).Debug($"{header.Key} = {header.Value.FirstOrDefault<string>()}");
        }

        Log.ForContext("SourceContext", _sourceContext).Debug(_exit);
        Log.ForContext("SourceContext", _sourceContext).Debug("Logging Content");
        Log.ForContext("SourceContext", _sourceContext).Debug(_exit);

        Log.ForContext("SourceContext", _sourceContext).Debug(await response.Content.ReadAsStringAsync());
    }
}