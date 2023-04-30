using ClassLibrary1;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace TestProject1;

[TestClass]
public class UnitTest1
{
    private const string appName = "app-prod-idso-000";

    [TestMethod]
    [TestCategory("IntegrationTest")]
    public async Task AppServiceNullStatusCode()
    {
        using var client = new HttpClient();
        using var response = await client.GetAsync($"https://{appName}.azurewebsites.net/");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    [TestCategory("IntegrationTest")]
    public async Task AppServiceNullContent()
    {
        using var client = new HttpClient();
        using var response = await client.GetAsync($"https://{appName}.azurewebsites.net/");

        Assert.IsFalse(Boolean.Parse(response.Content.ReadAsStringAsync().Result));
    }

    [TestMethod]
    [TestCategory("IntegrationTest")]
    public async Task AppServiceTrueStatusCode()
    {
        using var client = new HttpClient();
        using var response = await client.GetAsync($"https://{appName}.azurewebsites.net/?input=true");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    [TestCategory("IntegrationTest")]
    public async Task AppServiceTrueContent()
    {
        using var client = new HttpClient();
        using var response = await client.GetAsync($"https://{appName}.azurewebsites.net/?input=true");

        Assert.IsTrue(Boolean.Parse(response.Content.ReadAsStringAsync().Result));
    }

    [TestMethod]
    [TestCategory("IntegrationTest")]
    public async Task AppServiceFalseStatusCode()
    {
        using var client = new HttpClient();
        using var response = await client.GetAsync($"https://{appName}.azurewebsites.net/?input=false");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    [TestCategory("IntegrationTest")]
    public async Task AppServiceFalseContent()
    {
        using var client = new HttpClient();
        using var response = await client.GetAsync($"https://{appName}.azurewebsites.net/?input=false");

        Assert.IsFalse(Boolean.Parse(response.Content.ReadAsStringAsync().Result));
    }

    [TestMethod]
    [TestCategory("IntegrationTest")]
    public async Task KestrelNullStatusCode()
    {
        using var client = new HttpClient();
        using var response = await client.GetAsync("https://localhost:444/");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    [TestCategory("IntegrationTest")]
    public async Task KestrelNullContent()
    {
        using var client = new HttpClient();
        using var response = await client.GetAsync("https://localhost:444/");

        Assert.IsFalse(Boolean.Parse(response.Content.ReadAsStringAsync().Result));
    }

    [TestMethod]
    [TestCategory("IntegrationTest")]
    public async Task KestrelTrueStatusCode()
    {
        using var client = new HttpClient();
        using var response = await client.GetAsync("https://localhost:444/?input=true");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    [TestCategory("IntegrationTest")]
    public async Task KestrelTrueContent()
    {
        using var client = new HttpClient();
        using var response = await client.GetAsync("https://localhost:444/?input=true");

        Assert.IsTrue(Boolean.Parse(response.Content.ReadAsStringAsync().Result));
    }

    [TestMethod]
    [TestCategory("IntegrationTest")]
    public async Task KestrelFalseStatusCode()
    {
        using var client = new HttpClient();
        using var response = await client.GetAsync("https://localhost:444/?input=false");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    [TestCategory("IntegrationTest")]
    public async Task KestrelFalseContent()
    {
        using var client = new HttpClient();
        using var response = await client.GetAsync("https://localhost:444/?input=false");

        Assert.IsFalse(Boolean.Parse(response.Content.ReadAsStringAsync().Result));
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public async Task InProcessNullStatusCode()
    {
        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();
        using var response = await client.GetAsync("/");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public async Task InProcessNullContent()
    {
        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();
        using var response = await client.GetAsync("/");

        Assert.IsFalse(Boolean.Parse(response.Content.ReadAsStringAsync().Result));
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public async Task InProcessTrueStatusCode()
    {
        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();
        using var response = await client.GetAsync("/?input=true");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public async Task InProcessTrueContent()
    {
        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();
        using var response = await client.GetAsync("/?input=true");

        Assert.IsTrue(Boolean.Parse(response.Content.ReadAsStringAsync().Result));
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public async Task InProcessFalseStatusCode()
    {
        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();
        using var response = await client.GetAsync("/?input=false");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public async Task InProcessFalseContent()
    {
        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();
        using var response = await client.GetAsync("/?input=false");

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