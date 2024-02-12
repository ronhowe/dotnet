/*******************************************************************************
https://github.com/ronhowe
*******************************************************************************/

using ClassLibrary1;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using System.Net;

namespace TestProject1;

[TestClass]
public class IntegrationTests : TestBase
{
    [TestMethod]
    public async Task ApplicationHeaderIsValid()
    {
        WriteStartingWebApplicationToLogger();

        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();

        WriteBeginningHttpRequestToLogger();

        using var response = await client.GetAsync($"{Service1Endpoint.Service1}?input={Boolean.FalseString}");

        WriteEndingHttpRequestToLogger();

        if (response.Headers.TryGetValues("CustomHeader", out var values))
        {
            values.First().Should<string>().Be("default");
        }
    }

    [TestMethod]
    public async Task ApplicationThrowsFromNullInput()
    {
        WriteStartingWebApplicationToLogger();

        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();

        WriteBeginningHttpRequestToLogger();

        using var response = await client.GetAsync(Service1Endpoint.Service1);

        WriteEndingHttpRequestToLogger();

        response.StatusCode.Should<HttpStatusCode>().Be(HttpStatusCode.BadRequest);
    }

    [TestMethod]
    public async Task ApplicationRespondsNotFoundFromInvalidRoute()
    {
        WriteStartingWebApplicationToLogger();

        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();

        WriteBeginningHttpRequestToLogger();

        using var response = await client.GetAsync("");

        WriteEndingHttpRequestToLogger();

        response.StatusCode.Should<HttpStatusCode>().Be(HttpStatusCode.NotFound);
    }

    [TestMethod]
    public async Task ApplicationRespondsOKFromTrueInput()
    {
        WriteStartingWebApplicationToLogger();

        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();

        WriteBeginningHttpRequestToLogger();

        using var response = await client.GetAsync($"{Service1Endpoint.Service1}?input={Boolean.TrueString}");

        WriteEndingHttpRequestToLogger();

        response.StatusCode.Should<HttpStatusCode>().Be(HttpStatusCode.OK);
    }

    [TestMethod]
    public async Task ApplicationReturnsTrueFromTrueInput()
    {
        WriteStartingWebApplicationToLogger();

        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });

        WriteBeginningHttpRequestToLogger();

        using var client = application.CreateClient();
        using var response = await client.GetAsync($"{Service1Endpoint.Service1}?input={Boolean.TrueString}");


        WriteEndingHttpRequestToLogger();

        Boolean.Parse(response.Content.ReadAsStringAsync().Result).Should().BeTrue();
    }

    [TestMethod]
    public async Task ApplicationRespondsOKFromFalseInput()
    {
        WriteStartingWebApplicationToLogger();

        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();

        WriteBeginningHttpRequestToLogger();

        using var response = await client.GetAsync($"{Service1Endpoint.Service1}?input={Boolean.FalseString}");

        WriteEndingHttpRequestToLogger();

        response.StatusCode.Should<HttpStatusCode>().Be(HttpStatusCode.OK);
    }

    [TestMethod]
    public async Task ApplicationReturnsFalseFromFalseInput()
    {
        WriteStartingWebApplicationToLogger();

        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();

        WriteBeginningHttpRequestToLogger();

        using var response = await client.GetAsync($"{Service1Endpoint.Service1}?input={Boolean.FalseString}");

        WriteEndingHttpRequestToLogger();

        Boolean.Parse(response.Content.ReadAsStringAsync().Result).Should().BeFalse();
    }

    [TestMethod]
    public async Task HealthCheckHeaderIsValid()
    {
        WriteStartingWebApplicationToLogger();

        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();

        WriteBeginningHttpRequestToLogger();

        using var response = await client.GetAsync(Service1Endpoint.HealthCheck);

        WriteEndingHttpRequestToLogger();

        if (response.Headers.TryGetValues("CustomHeader", out var values))
        {
            values.First().Should<string>().Be("default");
        }
    }

    [TestMethod]
    public async Task HealthCheckRespondsOK()
    {
        WriteStartingWebApplicationToLogger();

        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();

        WriteBeginningHttpRequestToLogger();

        using var response = await client.GetAsync(Service1Endpoint.HealthCheck);

        WriteEndingHttpRequestToLogger();

        response.StatusCode.Should<HttpStatusCode>().Be(HttpStatusCode.OK);
    }

    [TestMethod]
    public async Task HealthCheckReturnsHealthy()
    {
        WriteStartingWebApplicationToLogger();

        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();

        WriteBeginningHttpRequestToLogger();

        using var response = await client.GetAsync(Service1Endpoint.HealthCheck);

        WriteEndingHttpRequestToLogger();

        response.Content.ReadAsStringAsync().Result.Should<string>().Be("Healthy");
    }

    [TestMethod]
    public async Task HealthCheckThrowsMockServiceException()
    {
        WriteStartingWebApplicationToLogger();

        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, configBuilder) =>
            {
                configBuilder.AddInMemoryCollection(new Dictionary<string, string?> { { "FeatureManagement:MockService1PermanentExceptionToggle", "true" } });
            });
        });
        using var client = application.CreateClient();

        WriteBeginningHttpRequestToLogger();

        using var response = await client.GetAsync(Service1Endpoint.HealthCheck);

        WriteEndingHttpRequestToLogger();

        response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
    }

    private void WriteStartingWebApplicationToLogger()
    {
        Log.ForContext("SourceContext", _sourceContext).Debug("Starting Web Application");
    }

    private void WriteBeginningHttpRequestToLogger()
    {
        Log.ForContext("SourceContext", _sourceContext).Debug("Beginning HTTP Request");
    }

    private void WriteEndingHttpRequestToLogger()
    {
        Log.ForContext("SourceContext", _sourceContext).Debug("Ending HTTP Request");
    }
}