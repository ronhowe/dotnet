using ClassLibrary1;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using Serilog.Events;
using System.Net;

namespace TestProject1;

[TestClass]
public class WebApplication1Tests
{
    private const string contextValue = nameof(WebApplication1Tests);

    [TestInitialize]
    public void TestInitialize()
    {
        const string outputTemplate = "{SourceContext} @ {Message}{NewLine}{Exception}";

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .WriteTo.Console(outputTemplate: outputTemplate)
            .CreateLogger();

        Log.ForContext("SourceContext", contextValue).Information("Running Integration Test");
    }

    [TestMethod]
    public async Task ApplicationHeaderExists()
    {
        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();
        using var response = await client.GetAsync(Service1Endpoint.Service1);

        foreach (var header in response.Headers)
        {
            Log.ForContext("SourceContext", contextValue).Information($"{header.Key}={header.Value.First()}");
        }

        Assert.IsTrue(response.Headers.Contains("CustomHeader"));
        response.Headers.Contains("CustomHeader").Should().BeTrue();
    }

    [TestMethod]
    public async Task ApplicationHeaderIsCorrect()
    {
        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();
        using var response = await client.GetAsync(Service1Endpoint.Service1);

        foreach (var header in response.Headers)
        {
            Log.ForContext("SourceContext", contextValue).Information($"{header.Key}={header.Value.First()}");
        }

        if (response.Headers.TryGetValues("CustomHeader", out var values))
        {
            Assert.AreEqual<string>("default", values.First());
            values.First().Should<string>().Be("default");
        }
    }

    [TestMethod]
    public async Task ApplicationRespondsOKFromNullInput()
    {
        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();
        using var response = await client.GetAsync(Service1Endpoint.Service1);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        response.StatusCode.Should<HttpStatusCode>().Be(HttpStatusCode.OK);
    }

    [TestMethod]
    public async Task ApplicationReturnsFalseFromNullInput()
    {
        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();
        using var response = await client.GetAsync(Service1Endpoint.Service1);

        Assert.IsFalse(Boolean.Parse(response.Content.ReadAsStringAsync().Result));
        Boolean.Parse(response.Content.ReadAsStringAsync().Result).Should().BeFalse();
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
                configBuilder.AddInMemoryCollection(new Dictionary<string, string?> { { "MockService1ExceptionToggle", "true" } });
            });
        });

        using var client = application.CreateClient();
        using var response = await client.GetAsync(Service1Endpoint.Service1);

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
    public async Task HealthCheckHeaderExists()
    {
        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();
        using var response = await client.GetAsync(Service1Endpoint.HealthCheck);

        foreach (var header in response.Headers)
        {
            Log.ForContext("SourceContext", contextValue).Information($"{header.Key}={header.Value.First()}");
        }

        Assert.IsTrue(response.Headers.Contains("CustomHeader"));
        response.Headers.Contains("CustomHeader").Should().BeTrue();
    }

    [TestMethod]
    public async Task HealthCheckHeaderIsCorrect()
    {
        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();
        using var response = await client.GetAsync(Service1Endpoint.HealthCheck);

        foreach (var header in response.Headers)
        {
            Log.ForContext("SourceContext", contextValue).Information($"{header.Key}={header.Value.First()}");
        }

        if (response.Headers.TryGetValues("CustomHeader", out var values))
        {
            Assert.AreEqual<string>("default", values.First());
            values.First().Should<string>().Be("default");
        }
    }
}