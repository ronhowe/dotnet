using ClassLibrary1;
using System.Diagnostics;

namespace TestProject1;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void TestMethod1()
    {
        Trace.WriteLine(DateTime.Now.ToString());
        var class1 = new Class1();
        Assert.IsTrue(class1.Method1(true));
        Assert.IsFalse(class1.Method1(false));
        Assert.IsTrue(class1.Method1(!false));
        Assert.IsFalse(class1.Method1(!true));
    }
}