using ClassLibrary1;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace TestProject1;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public async Task IntegrationTestMethod1()
    {
        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();

        HttpResponseMessage response;

        response = await client.GetAsync("/");
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsFalse(Boolean.Parse(response.Content.ReadAsStringAsync().Result));

        //response = await client.GetAsync("/?input=");
        //Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        //Assert.IsTrue(Boolean.Parse(response.Content.ReadAsStringAsync().Result));

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

        Assert.IsTrue(class1.Method1(true));
        Assert.IsFalse(class1.Method1(false));
    }
}