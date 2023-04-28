using ClassLibrary1;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Net;

namespace TestProject1;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public async Task TestMethod1()
    {
        Trace.WriteLine(DateTime.Now.ToString());
        var class1 = new Class1();
        Assert.IsTrue(class1.Method1(true));
        Assert.IsFalse(class1.Method1(false));
        Assert.IsTrue(class1.Method1(!false));
        Assert.IsFalse(class1.Method1(!true));

        using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        using var client = application.CreateClient();

        var response = await client.GetAsync("/");
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }
}