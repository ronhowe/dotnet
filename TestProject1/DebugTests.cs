/*******************************************************************************
https://github.com/ronhowe/dotnet
*******************************************************************************/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace TestProject1;

[TestClass]
public class DebugTests : TestBase
{
    [TestMethod]
    public void POST()
    {
        Debug.WriteLine("POST");

        Assert.IsTrue(true);
    }
}