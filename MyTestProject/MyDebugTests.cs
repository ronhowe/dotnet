using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Management.Automation;
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

        Debug.WriteLine($"Running PowerShell Script");
        using PowerShell ps = PowerShell.Create();

        ps.AddScript("$DebugPreference = 'Continue';");
        ps.AddScript("$ErrorPreference = 'Continue';");
        ps.AddScript("$InformationPreference = 'Continue';");
        ps.AddScript("$VerbosePreference = 'Continue';");
        ps.AddScript("$WarningPreference = 'Continue';");
        ps.AddScript("Set-ExecutionPolicy -ExecutionPolicy Unrestricted -Scope Process;");
        ps.AddScript(@"D:\repos\ronhowe\powershell\script\Debug-AzureAutomationRunbook.ps1");

        var results = ps.Invoke();

        // Write-Output
        foreach (var result in results)
        {
            if (result != null)
            {
                Debug.WriteLine(result);
            }
        }

        // Write-Host & Write-Information
        foreach (var information in ps.Streams.Information)
        {
            Debug.WriteLine($"Information: {information}");
        }

        // Write-Error
        foreach (var error in ps.Streams.Error)
        {
            Debug.WriteLine($"Error: {error}");
        }

        // Write-Warning
        foreach (var warning in ps.Streams.Warning)
        {
            Debug.WriteLine($"Warning: {warning}");
        }

        // Write-Verbose
        foreach (var verbose in ps.Streams.Verbose)
        {
            Debug.WriteLine($"Verbose: {verbose}");
        }

        // Write-Debug
        foreach (var debug in ps.Streams.Debug)
        {
            Debug.WriteLine($"Debug: {debug}");
        }
    }
}
