using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace TestProject1;

[TestClass]
public class IntegrationTest1
{
    private const string appName = "app-ronhowe-000";

    [TestMethod]
    [Ignore]
    public async Task AppServiceNullStatusCode()
    {
        using var client = new HttpClient();
        using var response = await client.GetAsync($"https://{appName}.azurewebsites.net/");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    [Ignore]
    public async Task AppServiceNullContent()
    {
        using var client = new HttpClient();
        using var response = await client.GetAsync($"https://{appName}.azurewebsites.net/");

        Assert.IsFalse(Boolean.Parse(response.Content.ReadAsStringAsync().Result));
    }

    [TestMethod]
    [Ignore]
    public async Task AppServiceTrueStatusCode()
    {
        using var client = new HttpClient();
        using var response = await client.GetAsync($"https://{appName}.azurewebsites.net/?input=true");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    [Ignore]
    public async Task AppServiceTrueContent()
    {
        using var client = new HttpClient();
        using var response = await client.GetAsync($"https://{appName}.azurewebsites.net/?input=true");

        Assert.IsTrue(Boolean.Parse(response.Content.ReadAsStringAsync().Result));
    }

    [TestMethod]
    [Ignore]
    public async Task AppServiceFalseStatusCode()
    {
        using var client = new HttpClient();
        using var response = await client.GetAsync($"https://{appName}.azurewebsites.net/?input=false");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    [Ignore]
    public async Task AppServiceFalseContent()
    {
        using var client = new HttpClient();
        using var response = await client.GetAsync($"https://{appName}.azurewebsites.net/?input=false");

        Assert.IsFalse(Boolean.Parse(response.Content.ReadAsStringAsync().Result));
    }

    [TestMethod]
    [Ignore]
    public async Task KestrelNullStatusCode()
    {
        using var client = new HttpClient();
        using var response = await client.GetAsync("https://localhost:444/");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    [Ignore]
    public async Task KestrelNullContent()
    {
        using var client = new HttpClient();
        using var response = await client.GetAsync("https://localhost:444/");

        Assert.IsFalse(Boolean.Parse(response.Content.ReadAsStringAsync().Result));
    }

    [TestMethod]
    [Ignore]
    public async Task KestrelTrueStatusCode()
    {
        using var client = new HttpClient();
        using var response = await client.GetAsync("https://localhost:444/?input=true");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    [Ignore]
    public async Task KestrelTrueContent()
    {
        using var client = new HttpClient();
        using var response = await client.GetAsync("https://localhost:444/?input=true");

        Assert.IsTrue(Boolean.Parse(response.Content.ReadAsStringAsync().Result));
    }

    [TestMethod]
    [Ignore]
    public async Task KestrelFalseStatusCode()
    {
        using var client = new HttpClient();
        using var response = await client.GetAsync("https://localhost:444/?input=false");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    [Ignore]
    public async Task KestrelFalseContent()
    {
        using var client = new HttpClient();
        using var response = await client.GetAsync("https://localhost:444/?input=false");

        Assert.IsFalse(Boolean.Parse(response.Content.ReadAsStringAsync().Result));
    }
}