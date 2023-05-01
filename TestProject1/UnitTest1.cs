using ClassLibrary1;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace TestProject1;

[TestClass]
public class UnitTest1
{
    private const string _configurationKey = "MockExceptionEnabled";

    [TestMethod]
    public void ServiceInputNull()
    {
        Assert.IsFalse(CreateServiceWithMockDependencies().Run(null));
    }

    [TestMethod]
    public void ServiceInputTrue()
    {
        Assert.IsTrue(CreateServiceWithMockDependencies().Run(true));
    }

    [TestMethod]
    public void ServiceInputFalse()
    {
        Assert.IsFalse(CreateServiceWithMockDependencies().Run(false));
    }

    [TestMethod]
    public void Logging()
    {
        var mockLogger = new Mock<ILogger<Service>>();

        var service = new Service(mockLogger.Object, CreateMockConfiguration(false));

        service.Run(false);

        mockLogger.VerifyInformationWasCalled(false.ToString());
    }

    [TestMethod]
    public void Configuration()
    {
        var service = new Service(CreateMockLogger(), CreateMockConfiguration(true));

        Assert.ThrowsException<NotImplementedException>(() => service.Run(null));
    }

    private static ILogger<Service> CreateMockLogger()
    {
        var logger = new Mock<ILogger<Service>>();

        return logger.Object;
    }

    private static IConfiguration CreateMockConfiguration(bool value)
    {
        var mockConfigurationSection = new Mock<IConfigurationSection>();
        mockConfigurationSection.Setup(x => x.Value).Returns(value.ToString());

        var mockConfiguration = new Mock<IConfiguration>();
        mockConfiguration.Setup(x => x.GetSection(_configurationKey)).Returns(mockConfigurationSection.Object);

        return mockConfiguration.Object;
    }

    private static Service CreateServiceWithMockDependencies()
    {
        var service = new Service(CreateMockLogger(), CreateMockConfiguration(false));

        return service;
    }

}