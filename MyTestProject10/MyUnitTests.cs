using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Moq;
using MyClassLibrary10;
using Shouldly;
using System.Diagnostics;

namespace MyTestProject10;

[TestClass]
public sealed class MyUnitTests : TestBase
{
    [TestMethod]
    [TestCategory("UnitTest")]
    [DataRow(false)]
    [DataRow(true)]
    public async Task MyServiceTests(bool value)
    {
        Debug.WriteLine($"Mocking {nameof(ILogger<MyService>)}");
        Mock<ILogger<MyService>> mockLogger = new();

        Debug.WriteLine($"Mocking {nameof(IConfiguration)}");
        Mock<IConfiguration> mockConfiguration = new();
        mockConfiguration.Setup(x => x["ConnectionStrings.MyDatabase"]).Returns("MYMOCKDATABASECONNECTIONSTRING");
        mockConfiguration.Setup(x => x["ConnectionStrings.MyAzureStorage"]).Returns("MYMOCKAZURESTORAGECONNECTIONSTRING;");
        mockConfiguration.Setup(x => x["MySecret"]).Returns("MYMOCKSECRET");

        Debug.WriteLine($"Mocking {nameof(IFeatureManager)}");
        Mock<IFeatureManager> mockFeatureManager = new();
        mockFeatureManager.Setup(x => x.IsEnabledAsync("MyFeature").Result).Returns(value);

        Debug.WriteLine($"Mocking {nameof(IMyRepository)}");
        Mock<IMyRepository> mockRepository = new();

        Debug.WriteLine($"Creating {nameof(MyService)}");
        MyService myService = new(
            mockLogger.Object,
            mockConfiguration.Object,
            mockFeatureManager.Object,
            mockRepository.Object
        );

        Debug.WriteLine($"Calling {nameof(MyService)} With {value}");
        bool result = await myService.MyMethodAsync(value);

        Debug.WriteLine($"Asserting Result Is {value}");
        result.ShouldBe(value);

        Debug.WriteLine($"Asserting Log Message Exists For Enter");
        mockLogger.VerifyLogMessage($"Entering {nameof(MyService)}", LogLevel.Information);

        Debug.WriteLine($"Asserting Log Message Exists For Input");
        mockLogger.VerifyLogMessage($"myInput = {value}", LogLevel.Debug);

        Debug.WriteLine($"Asserting Log Message Exists For Returning");
        mockLogger.VerifyLogMessage($"Returning {value}", LogLevel.Information);

        Debug.WriteLine($"Asserting Log Message Exists For Exiting");
        mockLogger.VerifyLogMessage($"Exiting {nameof(MyService)}", LogLevel.Information);
    }
}
