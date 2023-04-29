using ClassLibrary1;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace TestProject1;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    [Ignore]
    public async Task PublicIntegrationTestMethod1()
    {
        using var client = new HttpClient();

        HttpResponseMessage response;

        var endpoint = "TBD";

        response = await client.GetAsync($"https://{endpoint}/");
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsFalse(Boolean.Parse(response.Content.ReadAsStringAsync().Result));

        response = await client.GetAsync($"https://{endpoint}/?input=true");
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsTrue(Boolean.Parse(response.Content.ReadAsStringAsync().Result));

        response = await client.GetAsync($"https://{endpoint}/?input=false");
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsFalse(Boolean.Parse(response.Content.ReadAsStringAsync().Result));
    }

    [TestMethod]
    [Ignore]
    public async Task FrontDoorIntegrationTestMethod1()
    {
        using var client = new HttpClient();

        HttpResponseMessage response;

        var endpoint = "TBD";

        response = await client.GetAsync($"https://{endpoint}/");
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsFalse(Boolean.Parse(response.Content.ReadAsStringAsync().Result));

        response = await client.GetAsync($"https://{endpoint}/?input=true");
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsTrue(Boolean.Parse(response.Content.ReadAsStringAsync().Result));

        response = await client.GetAsync($"https://{endpoint}/?input=false");
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsFalse(Boolean.Parse(response.Content.ReadAsStringAsync().Result));
    }

    [TestMethod]
    [Ignore]
    public async Task AppServiceIntegrationTestMethod1()
    {
        using var client = new HttpClient();

        HttpResponseMessage response;

        var endpoint = "TBD";

        response = await client.GetAsync($"https://{endpoint}/");
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsFalse(Boolean.Parse(response.Content.ReadAsStringAsync().Result));

        response = await client.GetAsync($"https://{endpoint}/?input=true");
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsTrue(Boolean.Parse(response.Content.ReadAsStringAsync().Result));

        response = await client.GetAsync($"https://{endpoint}/?input=false");
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsFalse(Boolean.Parse(response.Content.ReadAsStringAsync().Result));
    }

    [TestMethod]
    [Ignore]
    public async Task KestrelIntegrationTestMethod1()
    {
        using var client = new HttpClient();

        HttpResponseMessage response;

        var endpoint = "localhost:444";

        response = await client.GetAsync($"https://{endpoint}/");
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsFalse(Boolean.Parse(response.Content.ReadAsStringAsync().Result));

        response = await client.GetAsync($"https://{endpoint}/?input=true");
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsTrue(Boolean.Parse(response.Content.ReadAsStringAsync().Result));

        response = await client.GetAsync($"https://{endpoint}/?input=false");
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsFalse(Boolean.Parse(response.Content.ReadAsStringAsync().Result));
    }

    [TestMethod]
    public async Task InProcessIntegrationTestMethod1()
    {
        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();

        HttpResponseMessage response;

        response = await client.GetAsync("/");
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsFalse(Boolean.Parse(response.Content.ReadAsStringAsync().Result));

        response = await client.GetAsync("/?input=true");
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsTrue(Boolean.Parse(response.Content.ReadAsStringAsync().Result));

        response = await client.GetAsync("/?input=false");
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsFalse(Boolean.Parse(response.Content.ReadAsStringAsync().Result));
    }

    [TestMethod]
    public void UnitTestMethod1()
    {
        var class1 = new Class1();

        Assert.IsFalse(class1.Method1(null));
        Assert.IsTrue(class1.Method1(true));
        Assert.IsFalse(class1.Method1(false));
    }
}