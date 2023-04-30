using ClassLibrary1;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace TestProject1;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    [TestCategory("IntegrationTest")]
    public async Task AppServiceNull()
    {
        using var client = new HttpClient();
        var endpoint = "app-prod-idso-000.azurewebsites.net";
        using var response = await client.GetAsync($"https://{endpoint}/");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsFalse(Boolean.Parse(response.Content.ReadAsStringAsync().Result));
    }

    [TestMethod]
    [TestCategory("IntegrationTest")]
    public async Task AppServiceTrue()
    {
        using var client = new HttpClient();
        var endpoint = "app-prod-idso-000.azurewebsites.net";
        using var response = await client.GetAsync($"https://{endpoint}/?input=true");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsTrue(Boolean.Parse(response.Content.ReadAsStringAsync().Result));
    }

    [TestMethod]
    [TestCategory("IntegrationTest")]
    public async Task AppServiceFalse()
    {
        using var client = new HttpClient();
        var endpoint = "app-prod-idso-000.azurewebsites.net";
        using var response = await client.GetAsync($"https://{endpoint}/?input=false");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsFalse(Boolean.Parse(response.Content.ReadAsStringAsync().Result));
    }

    [TestMethod]
    [TestCategory("IntegrationTest")]
    public async Task KestrelNull()
    {
        using var client = new HttpClient();
        var endpoint = "localhost:444";
        using var response = await client.GetAsync($"https://{endpoint}/");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsFalse(Boolean.Parse(response.Content.ReadAsStringAsync().Result));
    }

    [TestMethod]
    [TestCategory("IntegrationTest")]
    public async Task KestrelTrue()
    {
        using var client = new HttpClient();
        var endpoint = "localhost:444";
        using var response = await client.GetAsync($"https://{endpoint}/?input=true");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsTrue(Boolean.Parse(response.Content.ReadAsStringAsync().Result));
    }

    [TestMethod]
    [TestCategory("IntegrationTest")]
    public async Task KestrelFalse()
    {
        using var client = new HttpClient();
        var endpoint = "localhost:444";
        using var response = await client.GetAsync($"https://{endpoint}/?input=false");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsFalse(Boolean.Parse(response.Content.ReadAsStringAsync().Result));
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public async Task InProcessNull()
    {
        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();
        using var response = await client.GetAsync("/");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsFalse(Boolean.Parse(response.Content.ReadAsStringAsync().Result));
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public async Task InProcessTrue()
    {
        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();
        using var response = await client.GetAsync("/?input=true");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsTrue(Boolean.Parse(response.Content.ReadAsStringAsync().Result));
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public async Task InProcessFalse()
    {
        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();
        using var response = await client.GetAsync("/?input=false");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsFalse(Boolean.Parse(response.Content.ReadAsStringAsync().Result));
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public void UnitTestNull()
    {
        var class1 = new Class1();

        Assert.IsFalse(class1.Method1(null));
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public void UnitTestTrue()
    {
        var class1 = new Class1();

        Assert.IsTrue(class1.Method1(true));
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public void UnitTestFalse()
    {
        var class1 = new Class1();

        Assert.IsFalse(class1.Method1(false));
    }
}