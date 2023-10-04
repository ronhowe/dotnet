/*******************************************************************************
https://github.com/ronhowe/dotnet
*******************************************************************************/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace TestProject1;

[TestClass]
public class DebugTests
{
    [TestMethod]
    public void POST()
    {
        Debug.WriteLine("Power-On Self-Test");

        Assert.IsTrue(true);
    }
}