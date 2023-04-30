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

        Assert.IsFalse(service.Run(null));
        loggerMock.VerifyDebugWasCalled(false.ToString());
    }

    [TestMethod]
    public void UnitTestTrue()
    {
        var loggerMock = new Mock<ILogger<Service>>();
        var service = new Service(loggerMock.Object);

        Assert.IsTrue(service.Run(true));
        loggerMock.VerifyDebugWasCalled(true.ToString());
    }

    [TestMethod]
    public void UnitTestFalse()
    {
        var loggerMock = new Mock<ILogger<Service>>();
        var service = new Service(loggerMock.Object);

        Assert.IsFalse(service.Run(false));
        loggerMock.VerifyDebugWasCalled(false.ToString());
    }
}