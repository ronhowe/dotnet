using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace MyTestProject;

public class TestBase
{
    [TestCleanup]
    public void TestCleanup()
    {
        Debug.WriteLine($"Cleaning Test");
    }

    [TestInitialize]
    public void TestInitialize()
    {
        Debug.WriteLine($"{new string('*', 80)}");
        Debug.WriteLine("https://github.com/ronhowe");
        Debug.WriteLine($"{new string('*', 80)}");
        Debug.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} (LOCAL)");
        Debug.WriteLine($"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff} (UTC)");
        Debug.WriteLine($"Initializing Test");
    }
}
