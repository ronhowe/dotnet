/*******************************************************************************
https://github.com/ronhowe/dotnet
*******************************************************************************/

using ClassLibrary1;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Diagnostics;

namespace TestProject1;

[TestClass]
public class UnitTests
{
    [TestInitialize]
    public void TestInitialize()
    {
        Debug.WriteLine("Initializing Test");
    }

    [TestMethod]
    public void Service1LogsEntryMessage()
    {
        var mockLogger = new Mock<ILogger<Service1>>();

        var service = new Service1(
            mockLogger.Object,
            CreateMockConfiguration(false),
            CreateMockFeatureManager(nameof(Service1Feature.MockService1PermanentExceptionToggle), false),
            new MockDateTimeService(true)
        );

        service.Run(false);

        mockLogger.VerifyLogInformation($"Entering {nameof(Service1)}");
    }

    [TestMethod]
    public void Service1LogsExitMessage()
    {
        var mockLogger = new Mock<ILogger<Service1>>();

        var service = new Service1(
            mockLogger.Object,
            CreateMockConfiguration(false),
            CreateMockFeatureManager(nameof(Service1Feature.MockService1PermanentExceptionToggle), false),
            new MockDateTimeService(true)
        );

        service.Run(false);

        mockLogger.VerifyLogInformation($"Exiting {nameof(Service1)}");
    }

    [TestMethod]
    public void Service1LogsInputParameters()
    {
        var mockLogger = new Mock<ILogger<Service1>>();

        var service = new Service1(
            mockLogger.Object,
            CreateMockConfiguration(false),
            CreateMockFeatureManager(nameof(Service1Feature.MockService1PermanentExceptionToggle), false),
            new MockDateTimeService(true)
        );

        service.Run(false);

        mockLogger.VerifyLogDebug($"$input = {Boolean.FalseString}");
    }

    [TestMethod]
    public void Service1ReturnsFalseFromFalseInput()
    {
        var service = CreateServiceWithMockDependencies();

        Assert.IsFalse(service.Run(false));
        service.Run(false).Should().BeFalse();
    }

    [TestMethod]
    public void Service1ReturnsTrueFromTrueInput()
    {
        var service = CreateServiceWithMockDependencies();

        Assert.IsTrue(service.Run(true));
        service.Run(true).Should().BeTrue();
    }

    [TestMethod]
    public void Service1ThrowsWhenMockService1PermanentExceptionToggleIsTrue()
    {
        var service = new Service1(
            CreateMockLogger(),
            CreateMockConfiguration(false),
            CreateMockFeatureManager(nameof(Service1Feature.MockService1PermanentExceptionToggle), true),
            new MockDateTimeService(false)
        );

        Assert.ThrowsException<MockService1Exception>(() => service.Run(false));
        service.Invoking(y => y.Run(false)).Should().Throw<MockService1Exception>().WithMessage("MockService1PermanentExceptionToggle");
    }

    [TestMethod]
    public void Service1ReturnsWhenMockService1TransientExceptionToggleIsTrueOnEvenTicks()
    {
        var service = new Service1(
            CreateMockLogger()
            , CreateMockConfiguration(false)
            , CreateMockFeatureManager(nameof(Service1Feature.MockService1TransientExceptionToggle), true)
            , new MockDateTimeService(true)
        );

        Assert.IsFalse(service.Run(false));
        service.Run(false).Should().BeFalse();
    }

    [TestMethod]
    public void Service1ThrowsWhenMockService1TransientExceptionToggleIsTrueOnOddTicks()
    {
        var service = new Service1(
            CreateMockLogger(),
            CreateMockConfiguration(false),
            CreateMockFeatureManager(nameof(Service1Feature.MockService1TransientExceptionToggle), true),
            new MockDateTimeService(false)
        );

        Assert.ThrowsException<MockService1Exception>(() => service.Run(false));
        service.Invoking(y => y.Run(false)).Should().Throw<MockService1Exception>().WithMessage("MockService1TransientExceptionToggle");
    }

    [TestMethod]
    public void Service1HealthCheckReturnsHealthy()
    {
        var registration = new HealthCheckRegistration(
            name: "MockHealthCheck",
            instance: CreateMockHealthCheck(),
            failureStatus: null,
            tags: new[] { "tags" }
        );

        var context = new HealthCheckContext
        {
            Registration = registration
        };

        var service = new Service1HealthCheck(
            CreateMockLogger(),
            CreateMockConfiguration(false),
            CreateMockFeatureManager(string.Empty, false),
            new MockDateTimeService(false)
        );

        var result = service.CheckHealthAsync(context).Result;

        Assert.AreEqual<HealthStatus>(HealthStatus.Healthy, result.Status);
        result.Status.Should<HealthStatus>().Be(HealthStatus.Healthy);
    }

    [TestMethod]
    public void Service1HealthCheckReturnsUnhealthyWhenMockService1PermanentExceptionToggleIsTrue()
    {
        var registration = new HealthCheckRegistration(
            name: "MockHealthCheck",
            instance: CreateMockHealthCheck(),
            failureStatus: null,
            tags: new[] { "tags" }
        );

        var context = new HealthCheckContext
        {
            Registration = registration
        };

        var service = new Service1HealthCheck(
            CreateMockLogger(),
            CreateMockConfiguration(true),
            CreateMockFeatureManager(nameof(Service1Feature.MockService1PermanentExceptionToggle), true),
            new MockDateTimeService(true)
        );

        var result = service.CheckHealthAsync(context).Result;

        Assert.AreEqual<HealthStatus>(HealthStatus.Unhealthy, result.Status);
        result.Status.Should<HealthStatus>().Be(HealthStatus.Unhealthy);
    }

    [TestMethod]
    public void Service1HealthCheckReturnsHealthyWhenMockService1TransientExceptionToggleIsTrueOnEvenTicks()
    {
        var registration = new HealthCheckRegistration(
            name: "MockHealthCheck",
            instance: CreateMockHealthCheck(),
            failureStatus: null,
            tags: new[] { "tags" }
        );

        var context = new HealthCheckContext
        {
            Registration = registration
        };

        var service = new Service1HealthCheck(
            CreateMockLogger(),
            CreateMockConfiguration(false),
            CreateMockFeatureManager(nameof(Service1Feature.MockService1TransientExceptionToggle), true),
            new MockDateTimeService(true) // even ticks
        );

        var result = service.CheckHealthAsync(context).Result;

        Assert.AreEqual<HealthStatus>(HealthStatus.Healthy, result.Status);
        result.Status.Should<HealthStatus>().Be(HealthStatus.Healthy);
    }

    [TestMethod]
    public void Service1HealthCheckReturnsUnhealthyWhenMockService1TransientExceptionToggleIsTrueOnOddTicks()
    {
        var registration = new HealthCheckRegistration(
            name: "MockHealthCheck",
            instance: CreateMockHealthCheck(),
            failureStatus: null,
            tags: new[] { "tags" }
        );

        var context = new HealthCheckContext
        {
            Registration = registration
        };

        var service = new Service1HealthCheck(
            CreateMockLogger(),
            CreateMockConfiguration(false),
            CreateMockFeatureManager(nameof(Service1Feature.MockService1TransientExceptionToggle), true),
            new MockDateTimeService(false) // odd ticks
        );

        var result = service.CheckHealthAsync(context).Result;

        Assert.AreEqual<HealthStatus>(HealthStatus.Unhealthy, result.Status);
        result.Status.Should<HealthStatus>().Be(HealthStatus.Unhealthy);
    }

    #region Mocks
    private static IConfiguration CreateMockConfiguration(bool value)
    {
        //link - https://adamstorr.azurewebsites.net/blog/mocking-ilogger-with-moq
        var mockConfigurationSection = new Mock<IConfigurationSection>();
        mockConfigurationSection.Setup(x => x.Value).Returns(value.ToString());

        var mockConfiguration = new Mock<IConfiguration>();
        mockConfiguration.Setup(x => x.GetSection(nameof(Service1Feature.MockService1PermanentExceptionToggle))).Returns(mockConfigurationSection.Object);
        mockConfiguration.Setup(x => x.GetSection(nameof(Service1Feature.MockService1TransientExceptionToggle))).Returns(mockConfigurationSection.Object);

        return mockConfiguration.Object;
    }

    //private static IDateTimeService CreateMockDateTimeService(bool even)
    //{
    //    long ticks = even ? (DateTime.UtcNow.Ticks / 2) * 2 : ((DateTime.UtcNow.Ticks / 2) * 2) + 1;
    //    DateTime dateTime = new(ticks);

    //    var mockDateTimeService = new Mock<IDateTimeService>();
    //    mockDateTimeService.Setup(x => x.Now).Returns(dateTime);

    //    return mockDateTimeService.Object;
    //}

    private static IHealthCheck CreateMockHealthCheck()
    {
        var mockHealthCheck = new Mock<IHealthCheck>();

        return mockHealthCheck.Object;
    }

    private static IFeatureManager CreateMockFeatureManager(string name, bool value)
    {
        var mockFeatureManager = new Mock<IFeatureManager>();
        mockFeatureManager.Setup(x => x.IsEnabledAsync(name).Result).Returns(value);

        return mockFeatureManager.Object;
    }

    private static ILogger<Service1> CreateMockLogger()
    {
        var mockLogger = new Mock<ILogger<Service1>>();

        return mockLogger.Object;
    }

    private static Service1 CreateServiceWithMockDependencies()
    {
        var service = new Service1(
            CreateMockLogger(),
            CreateMockConfiguration(false),
            CreateMockFeatureManager(string.Empty, false),
            new MockDateTimeService(false)
        );

        return service;
    }
    #endregion Mocks
}

//todo - refactor
//todo - remove pragmas, i have no idea what it means =)
internal static class TestHelpers
{
    public static Mock<ILogger<T>> VerifyLogDebug<T>(this Mock<ILogger<T>> logger, string expectedMessage)
    {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        Func<object, Type, bool> state = (v, t) => v.ToString().CompareTo(expectedMessage) == 0;
#pragma warning restore CS8602 // Dereference of a possibly null reference.

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

    public static Mock<ILogger<T>> VerifyLogInformation<T>(this Mock<ILogger<T>> logger, string expectedMessage)
    {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        Func<object, Type, bool> state = (v, t) => v.ToString().CompareTo(expectedMessage) == 0;
#pragma warning restore CS8602 // Dereference of a possibly null reference.

#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
        logger.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Information),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => state(v, t)),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

        return logger;
    }
}