using System.Diagnostics;

namespace TestProject1;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void TestMethod1()
    {
        Trace.WriteLine(DateTime.Now.ToString());
        Assert.IsTrue(true);
    }
}