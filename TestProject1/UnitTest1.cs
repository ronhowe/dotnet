using ClassLibrary1;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace TestProject1;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void ServiceReturnsFalseFromNullInput()
    {
        Assert.IsFalse(CreateServiceWithMockDependencies().Run(null));
    }

    [TestMethod]
    public void ServiceReturnsTrueFromTrueInput()
    {
        Assert.IsTrue(CreateServiceWithMockDependencies().Run(true));
    }

    [TestMethod]
    public void ServiceReturnsFalseFromFalseInput()
    {
        Assert.IsFalse(CreateServiceWithMockDependencies().Run(false));
    }

    [TestMethod]
    public void ServiceLogsDebugMessage()
    {
        var mockLogger = new Mock<ILogger<Service>>();

        var service = new Service(mockLogger.Object, CreateMockConfiguration(false), CreateMockFeatureManager(false));

        service.Run(false);

        mockLogger.VerifyDebugWasCalled($"input={Boolean.FalseString}");
    }

    [TestMethod]
    public void ServiceThrowsMockServiceException()
    {
        var service = new Service(CreateMockLogger(), CreateMockConfiguration(true), CreateMockFeatureManager(true));

        Assert.ThrowsException<MockServiceException>(() => service.Run(null));
    }

    private static ILogger<Service> CreateMockLogger()
    {
        var mockLogger = new Mock<ILogger<Service>>();

        return mockLogger.Object;
    }

    private static IConfiguration CreateMockConfiguration(bool value)
    {
        var mockConfigurationSection = new Mock<IConfigurationSection>();
        mockConfigurationSection.Setup(x => x.Value).Returns(value.ToString());

        var mockConfiguration = new Mock<IConfiguration>();
        mockConfiguration.Setup(x => x.GetSection(nameof(ServiceFeatures.MockServiceExceptionToggle))).Returns(mockConfigurationSection.Object);

        return mockConfiguration.Object;
    }

    private static Service CreateServiceWithMockDependencies()
    {
        var service = new Service(CreateMockLogger(), CreateMockConfiguration(false), CreateMockFeatureManager(false));

        return service;
    }

    private static IFeatureManager CreateMockFeatureManager(bool value)
    {
        var mockFeatureManager = new Mock<IFeatureManager>();
        mockFeatureManager.Setup(x => x.IsEnabledAsync(nameof(ServiceFeatures.MockServiceExceptionToggle)).Result).Returns(value);

        return mockFeatureManager.Object;
    }
}