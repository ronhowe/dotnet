/////////////////////////////////////////////////////////////////////////////80

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
public class WebApplication1Tests
{
    private readonly string separator = new('*', 160);

    [TestInitialize]
    public void TestInitialize()
    {
        Debug.WriteLine("Initializing Test");
    }

    [TestMethod]
    public void ClientRetries()
    {
        var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(1), retryCount: 5);

        var retryPolicy = Policy
            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode || r.StatusCode == HttpStatusCode.InternalServerError)
            .WaitAndRetryAsync(delay, (response, timeSpan, retryCount, context) =>
             {
                 Debug.WriteLine($"RETRY ATTEMPT # {retryCount} AFTER {timeSpan} SECONDS");
             });

        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, configBuilder) =>
            {
                configBuilder.AddInMemoryCollection(new Dictionary<string, string?> { { "MockService1TransientExceptionToggle", "true" } });
            });
        });

        using var client = application.CreateClient();

        using var response = retryPolicy.ExecuteAsync(async () =>
        {
            Debug.WriteLine(separator);
            return await client.GetAsync($"{Service1Endpoint.Service1}?input={Boolean.FalseString}");
        }).Result;

        Debug.WriteLine(separator);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        response.StatusCode.Should<HttpStatusCode>().Be(HttpStatusCode.OK);
    }

    [TestMethod]
    public async Task ApplicationHeaderIsValid()
    {
        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();
        using var response = await client.GetAsync($"{Service1Endpoint.Service1}?input={Boolean.FalseString}");

        if (response.Headers.TryGetValues("CustomHeader", out var values))
        {
            Assert.AreEqual<string>("default", values.First());
            values.First().Should<string>().Be("default");
        }
    }

    [TestMethod]
    public async Task ApplicationThrowsFromNullInput()
    {
        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();
        using var response = await client.GetAsync(Service1Endpoint.Service1);

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        response.StatusCode.Should<HttpStatusCode>().Be(HttpStatusCode.BadRequest);
    }

    [TestMethod]
    public async Task ApplicationRespondsOKFromTrueInput()
    {
        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();
        using var response = await client.GetAsync($"{Service1Endpoint.Service1}?input={Boolean.TrueString}");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        response.StatusCode.Should<HttpStatusCode>().Be(HttpStatusCode.OK);
    }

    [TestMethod]
    public async Task ApplicationReturnsTrueFromTrueInput()
    {
        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();
        using var response = await client.GetAsync($"{Service1Endpoint.Service1}?input={Boolean.TrueString}");

        Assert.IsTrue(Boolean.Parse(response.Content.ReadAsStringAsync().Result));
        Boolean.Parse(response.Content.ReadAsStringAsync().Result).Should().BeTrue();
    }

    [TestMethod]
    public async Task ApplicationRespondsOKFromFalseInput()
    {
        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();
        using var response = await client.GetAsync($"{Service1Endpoint.Service1}?input={Boolean.FalseString}");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        response.StatusCode.Should<HttpStatusCode>().Be(HttpStatusCode.OK);
    }

    [TestMethod]
    public async Task ApplicationReturnsFalseFromFalseInput()
    {
        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();
        using var response = await client.GetAsync($"{Service1Endpoint.Service1}?input={Boolean.FalseString}");

        Assert.IsFalse(Boolean.Parse(response.Content.ReadAsStringAsync().Result));
        Boolean.Parse(response.Content.ReadAsStringAsync().Result).Should().BeFalse();
    }

    [TestMethod]
    public async Task ApplicationThrowsMockServiceException()
    {
        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, configBuilder) =>
            {
                configBuilder.AddInMemoryCollection(new Dictionary<string, string?> { { "MockService1PermanentExceptionToggle", "true" } });
            });
        });

        using var client = application.CreateClient();
        using var response = await client.GetAsync($"{Service1Endpoint.Service1}?input={Boolean.FalseString}");

        Assert.AreEqual<HttpStatusCode>(HttpStatusCode.InternalServerError, response.StatusCode);
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [TestMethod]
    public async Task HealthCheckRespondsOK()
    {
        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();
        using var response = await client.GetAsync(Service1Endpoint.HealthCheck);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        response.StatusCode.Should<HttpStatusCode>().Be(HttpStatusCode.OK);
    }

    [TestMethod]
    public async Task HealthCheckReturnsHealthy()
    {
        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();
        using var response = await client.GetAsync(Service1Endpoint.HealthCheck);

        Assert.AreEqual<string>("Healthy", response.Content.ReadAsStringAsync().Result);
        response.Content.ReadAsStringAsync().Result.Should<string>().Be("Healthy");
    }

    [TestMethod]
    public async Task HealthCheckHeaderIsValid()
    {
        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();
        using var response = await client.GetAsync(Service1Endpoint.HealthCheck);

        if (response.Headers.TryGetValues("CustomHeader", out var values))
        {
            Assert.AreEqual<string>("default", values.First());
            values.First().Should<string>().Be("default");
        }
    }

    [TestMethod]
    public async Task HealthCheckThrowsMockServiceException()
    {
        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, configBuilder) =>
            {
                configBuilder.AddInMemoryCollection(new Dictionary<string, string?> { { "MockService1PermanentExceptionToggle", "true" } });
            });
        });

        using var client = application.CreateClient();
        using var response = await client.GetAsync(Service1Endpoint.HealthCheck);

        Assert.AreEqual<HttpStatusCode>(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
    }
}