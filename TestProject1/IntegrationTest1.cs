using ClassLibrary1;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace TestProject1;

[TestClass]
public class IntegrationTest1
{
    [TestMethod]
    public async Task ApplicationThrowsMockException()
    {
        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Integration");
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
    public async Task ApplicationReturnsFalseFromNullInput()
    {
        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();
        using var response = await client.GetAsync("/");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsFalse(Boolean.Parse(response.Content.ReadAsStringAsync().Result));
    }

    [TestMethod]
    public async Task ApplicationReturnsTrueFromTrueInput()
    {
        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();
        using var response = await client.GetAsync($"/?input={Boolean.TrueString}");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsTrue(Boolean.Parse(response.Content.ReadAsStringAsync().Result));
    }

    [TestMethod]
    public async Task ApplicationReturnsFalseFromFalseInput()
    {
        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();
        using var response = await client.GetAsync($"/?input={Boolean.FalseString}");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsFalse(Boolean.Parse(response.Content.ReadAsStringAsync().Result));
    }
}