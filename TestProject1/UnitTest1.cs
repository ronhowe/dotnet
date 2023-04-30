using ClassLibrary1;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace TestProject1;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public async Task InProcessNullStatusCode()
    {
        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();
        using var response = await client.GetAsync("/");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    public async Task InProcessNullContent()
    {
        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();
        using var response = await client.GetAsync("/");

        Assert.IsFalse(Boolean.Parse(response.Content.ReadAsStringAsync().Result));
    }

    [TestMethod]
    public async Task InProcessTrueStatusCode()
    {
        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();
        using var response = await client.GetAsync("/?input=true");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    public async Task InProcessTrueContent()
    {
        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();
        using var response = await client.GetAsync("/?input=true");

        Assert.IsTrue(Boolean.Parse(response.Content.ReadAsStringAsync().Result));
    }

    [TestMethod]
    public async Task InProcessFalseStatusCode()
    {
        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();
        using var response = await client.GetAsync("/?input=false");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    public async Task InProcessFalseContent()
    {
        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();
        using var response = await client.GetAsync("/?input=false");

        Assert.IsFalse(Boolean.Parse(response.Content.ReadAsStringAsync().Result));
    }

    [TestMethod]
    public void UnitTestNull()
    {
        var class1 = new Class1();

        Assert.IsFalse(class1.Method1(null));
    }

    [TestMethod]
    public void UnitTestTrue()
    {
        var class1 = new Class1();

        Assert.IsTrue(class1.Method1(true));
    }

    [TestMethod]
    public void UnitTestFalse()
    {
        var class1 = new Class1();

        Assert.IsFalse(class1.Method1(false));
    }
}