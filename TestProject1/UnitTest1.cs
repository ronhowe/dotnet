using ClassLibrary1;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace TestProject1;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void UnitTestNull()
    {
        var loggerMock = new Mock<ILogger<Service>>();
        var service = new Service(loggerMock.Object);

        Assert.IsFalse(service.IO(null));
        loggerMock.VerifyDebugWasCalled("POST");
    }

    [TestMethod]
    public void UnitTestTrue()
    {
        var loggerMock = new Mock<ILogger<Service>>();
        var service = new Service(loggerMock.Object);

        Assert.IsTrue(service.IO(true));
        loggerMock.VerifyDebugWasCalled("POST");
    }

    [TestMethod]
    public void UnitTestFalse()
    {
        var loggerMock = new Mock<ILogger<Service>>();
        var service = new Service(loggerMock.Object);

        Assert.IsFalse(service.IO(false));
        loggerMock.VerifyDebugWasCalled("POST");
    }
}