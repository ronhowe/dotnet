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
public class ClassLibrary1Tests
{
    private const string contextValue = nameof(ClassLibrary1Tests);

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

        Assert.ThrowsException<MockService1Exception>(() => service.Run(null));
        service.Invoking(y => y.Run(null)).Should().Throw<MockService1Exception>().WithMessage("MockService1ExceptionToggle");
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
        mockConfiguration.Setup(x => x.GetSection(nameof(Service1Feature.MockService1ExceptionToggle))).Returns(mockConfigurationSection.Object);

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
        mockFeatureManager.Setup(x => x.IsEnabledAsync(nameof(Service1Feature.MockService1ExceptionToggle)).Result).Returns(value);

        return mockFeatureManager.Object;
    }
}

internal static class TestHelpers
{
    public static Mock<ILogger<T>> VerifyDebugWasCalled<T>(this Mock<ILogger<T>> logger, string expectedMessage)
    {
        //help - https://adamstorr.azurewebsites.net/blog/mocking-ilogger-with-moq
        //todo - refactor and remove pragma
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        Func<object, Type, bool> state = (v, t) => v.ToString().CompareTo(expectedMessage) == 0;
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        //todo - refactor and remove pragma
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
        logger.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Debug),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => state(v, t)),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

        return logger;
    }
}