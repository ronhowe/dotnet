using ClassLibrary1;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject1;

[TestClass]
public class UnitTest1
{
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