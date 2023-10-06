/*******************************************************************************
https://github.com/ronhowe/dotnet
*******************************************************************************/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;

namespace TestProject1;

[TestClass]
public class DebugTests : TestBase
{
    [TestMethod]
    public void POST()
    {
        Log.ForContext("SourceContext", _sourceContext).Debug("POST");

        Assert.IsTrue(true);
    }
}