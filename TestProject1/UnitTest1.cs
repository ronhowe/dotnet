using ClassLibrary1;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Serilog;
using Serilog.Events;

namespace TestProject1;

[TestClass]
public class UnitTest1
{
    private const string contextValue = nameof(UnitTest1);

    [TestInitialize]
    public void TestInitialize()
    {
        const string outputTemplate = "{SourceContext} @ {Message}{NewLine}{Exception}";

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .WriteTo.Console(outputTemplate: outputTemplate)
            .CreateLogger();

        Log.ForContext("SourceContext", contextValue).Information("Running Unit Test");
    }

    [TestMethod]
    public void ServiceLogsDebugMessage()
    {
        var mockLogger = new Mock<ILogger<Service1>>();

        var service = new Service1(mockLogger.Object, CreateMockConfiguration(false), CreateMockFeatureManager(false));

        service.Run(false);

        mockLogger.VerifyDebugWasCalled($"input={Boolean.FalseString}");
    }

    [TestMethod]
    public void ServiceReturnsFalseFromNullInput()
    {
        Assert.IsFalse(CreateServiceWithMockDependencies().Run(null));
        CreateServiceWithMockDependencies().Run(null).Should().BeFalse();
    }

    [TestMethod]
    public void ServiceReturnsTrueFromTrueInput()
    {
        Assert.IsTrue(CreateServiceWithMockDependencies().Run(true));
        CreateServiceWithMockDependencies().Run(true).Should().BeTrue();
    }

    [TestMethod]
    public void ServiceReturnsFalseFromFalseInput()
    {
        Assert.IsFalse(CreateServiceWithMockDependencies().Run(false));
        CreateServiceWithMockDependencies().Run(false).Should().BeFalse();
    }

    [TestMethod]
    public void ServiceThrowsMockServiceException()
    {
        var service = new Service1(CreateMockLogger(), CreateMockConfiguration(true), CreateMockFeatureManager(true));

        Assert.ThrowsException<MockServiceException>(() => service.Run(null));
        service.Invoking(y => y.Run(null)).Should().Throw<MockServiceException>().WithMessage("MockServiceExceptionToggle");
    }

    private static ILogger<Service1> CreateMockLogger()
    {
        var mockLogger = new Mock<ILogger<Service1>>();

        return mockLogger.Object;
    }

    private static IConfiguration CreateMockConfiguration(bool value)
    {
        //link - https://adamstorr.azurewebsites.net/blog/mocking-ilogger-with-moq
        var mockConfigurationSection = new Mock<IConfigurationSection>();
        mockConfigurationSection.Setup(x => x.Value).Returns(value.ToString());

        var mockConfiguration = new Mock<IConfiguration>();
        mockConfiguration.Setup(x => x.GetSection(nameof(ServiceFeatures.MockServiceExceptionToggle))).Returns(mockConfigurationSection.Object);

        return mockConfiguration.Object;
    }

    private static Service1 CreateServiceWithMockDependencies()
    {
        var service = new Service1(CreateMockLogger(), CreateMockConfiguration(false), CreateMockFeatureManager(false));

        return service;
    }

    private static IFeatureManager CreateMockFeatureManager(bool value)
    {
        var mockFeatureManager = new Mock<IFeatureManager>();
        mockFeatureManager.Setup(x => x.IsEnabledAsync(nameof(ServiceFeatures.MockServiceExceptionToggle)).Result).Returns(value);

        return mockFeatureManager.Object;
    }
}
