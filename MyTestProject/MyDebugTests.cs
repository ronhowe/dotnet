using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Security.Cryptography;

namespace MyTestProject;

[TestClass]
public sealed class MyDebugTests : TestBase
{
    [TestMethod]
    [TestCategory("DebugTest")]
    public void MyDebugTest()
    {
        Debug.WriteLine($"Debugging");

#if DEBUG
        Debug.WriteLine($"Defining DEBUG");
#endif

        Debug.WriteLine($"Creating Globally Unique Identifier");
        Debug.WriteLine(Guid.CreateVersion7());

        Debug.WriteLine($"Generating Random Number");
        byte[] key = new byte[4096 / 8];
        RandomNumberGenerator.Fill(key);
        string base64Key = Convert.ToBase64String(key);
        Debug.WriteLine(base64Key);
    }
}
