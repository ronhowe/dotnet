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
using Serilog.Events;
using System.Diagnostics;
using System.Net;

namespace TestProject1;

[TestClass]
public class TipTests
{
    private readonly string _asterisk = new('*', 80);
    private readonly string _enter = new('>', 80);
    private readonly string _exit = new('<', 80);
    private readonly string _sourceContext = nameof(IntegrationTests);
    private readonly string _outputTemplate = "[{SourceContext}] {Message}{NewLine}";

    [TestInitialize]
    public void TestInitialize()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .WriteTo.Console(outputTemplate: _outputTemplate)
            .CreateLogger();

        Log.ForContext("SourceContext", _sourceContext).Debug("Power-On Self-Test (1 of 5) => Logging Debug OK");
        Log.ForContext("SourceContext", _sourceContext).Information("Power-On Self-Test (2 of 5) => Logging Information OK");
        Log.ForContext("SourceContext", _sourceContext).Warning("Power-On Self-Test (3 of 5) => Logging Warning OK");
        Log.ForContext("SourceContext", _sourceContext).Error("Power-On Self-Test (4 of 5) => Logging Error OK");
        Log.ForContext("SourceContext", _sourceContext).Fatal("Power-On Self-Test (5 of 5) => Logging Fatal OK");

        Log.ForContext("SourceContext", _sourceContext).Debug(_asterisk);
        Log.ForContext("SourceContext", _sourceContext).Debug("Initializing Test");
        Log.ForContext("SourceContext", _sourceContext).Debug(_asterisk);
    }

    [TestMethod]
    [Ignore]
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
    public async Task ClientConnectsToAzureAppService000()
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
            //ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        using var client = new HttpClient(handler);

        using var response = retryPolicy.ExecuteAsync(async () =>
        {
            Debug.WriteLine(_enter);
            Debug.WriteLine("Starting HTTP Request");
            Debug.WriteLine(_enter);

            return await client.GetAsync("https://app-rhowe-000.azurewebsites.net/health");
        }).Result;

        Debug.WriteLine(_exit);
        Debug.WriteLine("Ending HTTP Request");
        Debug.WriteLine(_exit);

        Assert.AreEqual<HttpStatusCode>(HttpStatusCode.OK, response.StatusCode);
        response.StatusCode.Should<HttpStatusCode>().Be(HttpStatusCode.OK);

        foreach (var header in response.Headers)
        {
            Trace.WriteLine($"{header.Key} = {header.Value.FirstOrDefault<string>()}");
        }

        Debug.WriteLine(await response.Content.ReadAsStringAsync());
    }

    [TestMethod]
    [Ignore]
    public async Task ClientConnectsToAzureAppService001()
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

            return await client.GetAsync("https://app-ronhowe-001.azurewebsites.net");
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
    [Ignore]
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