/*******************************************************************************
https://github.com/ronhowe
*******************************************************************************/

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject1;

[TestClass]
public class DebugTests : TestBase
{
    [TestMethod]
    public void POST()
    {
        Console.WriteLine($"POST {DateTime.UtcNow}");
    }
}