using ClassLibrary1;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace TestProject1;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void ServiceInputNull()
    {
        var loggerMock = new Mock<ILogger<Service>>();
        var mockSection = new Mock<IConfigurationSection>();
        mockSection.Setup(x => x.Value).Returns(false.ToString());
        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(x => x.GetSection("MockServiceExceptionEnabled")).Returns(mockSection.Object);
        var service = new Service(loggerMock.Object, mockConfig.Object);

        Assert.IsFalse(service.Run(null));
    }

    [TestMethod]
    public void ServiceInputTrue()
    {
        var loggerMock = new Mock<ILogger<Service>>();
        var mockSection = new Mock<IConfigurationSection>();
        mockSection.Setup(x => x.Value).Returns(false.ToString());
        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(x => x.GetSection("MockServiceExceptionEnabled")).Returns(mockSection.Object);
        var service = new Service(loggerMock.Object, mockConfig.Object);

        Assert.IsTrue(service.Run(true));
    }

    [TestMethod]
    public void ServiceInputFalse()
    {
        var loggerMock = new Mock<ILogger<Service>>();
        var mockSection = new Mock<IConfigurationSection>();
        mockSection.Setup(x => x.Value).Returns(false.ToString());
        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(x => x.GetSection("MockServiceExceptionEnabled")).Returns(mockSection.Object);
        var service = new Service(loggerMock.Object, mockConfig.Object);

        Assert.IsFalse(service.Run(false));
    }

    [TestMethod]
    public void ServiceLogging()
    {
        var loggerMock = new Mock<ILogger<Service>>();
        var mockSection = new Mock<IConfigurationSection>();
        mockSection.Setup(x => x.Value).Returns(false.ToString());
        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(x => x.GetSection("MockServiceExceptionEnabled")).Returns(mockSection.Object);
        var service = new Service(loggerMock.Object, mockConfig.Object);

        service.Run(false);
        loggerMock.VerifyDebugWasCalled(false.ToString());
    }

    [TestMethod]
    public void ServiceConfiguration()
    {
        var loggerMock = new Mock<ILogger<Service>>();
        var mockSection = new Mock<IConfigurationSection>();
        mockSection.Setup(x => x.Value).Returns(true.ToString());
        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(x => x.GetSection("MockServiceExceptionEnabled")).Returns(mockSection.Object);
        var service = new Service(loggerMock.Object, mockConfig.Object);

        Assert.ThrowsException<NotImplementedException>(() => service.Run(null));
    }
}