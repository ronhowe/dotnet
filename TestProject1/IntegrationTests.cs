/*******************************************************************************
https://github.com/ronhowe/dotnet
*******************************************************************************/

using ClassLibrary1;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using Serilog.Events;
using System.Net;

namespace TestProject1;

[TestClass]
public class IntegrationTests
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
    public async Task ApplicationHeaderIsValid()
    {
        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });

        Log.ForContext("SourceContext", _sourceContext).Debug(_asterisk);
        Log.ForContext("SourceContext", _sourceContext).Debug("Starting Web Application");
        Log.ForContext("SourceContext", _sourceContext).Debug(_asterisk);

        using var client = application.CreateClient();

        Log.ForContext("SourceContext", _sourceContext).Debug(_enter);
        Log.ForContext("SourceContext", _sourceContext).Debug("Starting HTTP Request");
        Log.ForContext("SourceContext", _sourceContext).Debug(_enter);

        using var response = await client.GetAsync($"{Service1Endpoint.Service1}?input={Boolean.FalseString}");

        Log.ForContext("SourceContext", _sourceContext).Debug(_exit);
        Log.ForContext("SourceContext", _sourceContext).Debug("Ending HTTP Request");
        Log.ForContext("SourceContext", _sourceContext).Debug(_exit);

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
    public async Task ApplicationThrowsWhenMockService1PermanentExceptionToggleIsTrue()
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
    public async Task ApplicationThrowsWhenMockService1TransientExceptionToggleIsTrueOnOddTicks()
    {
        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, configBuilder) =>
            {
                configBuilder.AddInMemoryCollection(new Dictionary<string, string?> { { "MockService1TransientExceptionToggle", "true" } });
            });

            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton(MockDateTimeService.CreateMockDateTimeService(false));
            });
        });

        using var client = application.CreateClient();
        using var response = await client.GetAsync($"{Service1Endpoint.Service1}?input={Boolean.FalseString}");

        Assert.AreEqual<HttpStatusCode>(HttpStatusCode.InternalServerError, response.StatusCode);
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [TestMethod]
    public async Task ApplicationRespondsOKWhenMockService1TransientExceptionToggleIsTrueOnEvenTicks()
    {
        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, configBuilder) =>
            {
                configBuilder.AddInMemoryCollection(new Dictionary<string, string?> { { "MockService1TransientExceptionToggle", "true" } });
            });

            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton(MockDateTimeService.CreateMockDateTimeService(true));
            });
        });

        using var client = application.CreateClient();
        using var response = await client.GetAsync($"{Service1Endpoint.Service1}?input={Boolean.FalseString}");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        response.StatusCode.Should<HttpStatusCode>().Be(HttpStatusCode.OK);
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