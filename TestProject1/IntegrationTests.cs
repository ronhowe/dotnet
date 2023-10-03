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
using Moq;
using System.Diagnostics;
using System.Net;

namespace TestProject1;

[TestClass]
public class IntegrationTests
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
    public async Task ApplicationHeaderIsValid()
    {
        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });

        Debug.WriteLine(_asterisk);
        Debug.WriteLine("Starting Web Application");
        Debug.WriteLine(_asterisk);

        using var client = application.CreateClient();

        Debug.WriteLine(_enter);
        Debug.WriteLine("Starting HTTP Request");
        Debug.WriteLine(_enter);

        using var response = await client.GetAsync($"{Service1Endpoint.Service1}?input={Boolean.FalseString}");

        Debug.WriteLine(_exit);
        Debug.WriteLine("Ending HTTP Request");
        Debug.WriteLine(_exit);

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

            builder.ConfigureTestServices(services => {
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

            builder.ConfigureTestServices(services => {
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