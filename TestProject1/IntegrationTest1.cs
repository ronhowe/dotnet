using ClassLibrary1;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Net;

namespace TestProject1;

[TestClass]
public class IntegrationTest1
{
    [TestInitialize]
    public void TestInitialize()
    {
        Trace.TraceInformation("Running Integration Tests");
    }

    [TestMethod]
    public async Task ApplicationThrowsMockServiceException()
    {
        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Staging");

            Trace.TraceWarning("TODO - Mock Configuration In Integration Tests");
            // TODO - Example Code And Comments
            //builder.ConfigureAppConfiguration((context, configBuilder) =>
            //{
            //    //configBuilder.AddInMemoryCollection(
            //    //    new Dictionary<string, string?>
            //    //    {
            //    //        ["MockServiceExceptionToggle"] = "true"
            //    //    });
            //    //configBuilder.AddInMemoryCollection(new Dictionary<string, string?> { { "MockServiceExceptionToggle", "true" } });
            //});
        });

        using var client = application.CreateClient();

        try
        {
            using var response = await client.GetAsync("/");
            throw new Exception();
        }
        catch (Exception ex)
        {
            Assert.IsInstanceOfType<MockServiceException>(ex);
        }
    }

    [TestMethod]
    public async Task ApplicationRespondsOKFromNullInput()
    {
        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();
        using var response = await client.GetAsync("/");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    public async Task ApplicationReturnsFalseFromNullInput()
    {
        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();
        using var response = await client.GetAsync("/");

        Assert.IsFalse(Boolean.Parse(response.Content.ReadAsStringAsync().Result));
    }

    [TestMethod]
    public async Task ApplicationHeaderExists()
    {
        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();
        using var response = await client.GetAsync("/");

        // debugging
        foreach (var header in response.Headers)
        {
            Trace.TraceInformation($"{header.Key}={header.Value.First()}");
        }

        Assert.IsTrue(response.Headers.Contains("CustomHeader"));
    }

    [TestMethod]
    public async Task ApplicationHeaderIsCorrect()
    {
        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();
        using var response = await client.GetAsync("/");

        // debugging
        foreach (var header in response.Headers)
        {
            Trace.TraceInformation($"{header.Key}={header.Value.First()}");
        }

        if (response.Headers.TryGetValues("CustomHeader", out var values))
        {
            Assert.AreEqual<string>("default", values.First());
        }
    }

    [TestMethod]
    public async Task ApplicationRespondsOKFromTrueInput()
    {
        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();
        using var response = await client.GetAsync($"/?input={Boolean.TrueString}");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    public async Task ApplicationReturnsTrueFromTrueInput()
    {
        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();
        using var response = await client.GetAsync($"/?input={Boolean.TrueString}");

        Assert.IsTrue(Boolean.Parse(response.Content.ReadAsStringAsync().Result));
    }

    [TestMethod]
    public async Task ApplicationRespondsOKFromFalseInput()
    {
        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();
        using var response = await client.GetAsync($"/?input={Boolean.FalseString}");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    public async Task ApplicationReturnsFalseFromFalseInput()
    {
        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();
        using var response = await client.GetAsync($"/?input={Boolean.FalseString}");

        Assert.IsFalse(Boolean.Parse(response.Content.ReadAsStringAsync().Result));
    }
}