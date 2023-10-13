/*******************************************************************************
https://github.com/ronhowe/dotnet
*******************************************************************************/

using ClassLibrary1;
using FluentAssertions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace TestProject1;

[TestClass]
public class UnitTests : TestBase
{
    [TestMethod]
    public void Service1LogsEntryMessage()
    {
        var mockLogger = new Mock<ILogger<Service1>>();

        var service = new Service1(
            mockLogger.Object,
            TestMockFactory.CreateMockConfiguration(false),
            TestMockFactory.CreateMockFeatureManager(nameof(Service1Feature.MockService1PermanentExceptionToggle), false),
            TestMockFactory.CreateMockDateTimeService(true)
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
            TestMockFactory.CreateMockConfiguration(false),
            TestMockFactory.CreateMockFeatureManager(nameof(Service1Feature.MockService1PermanentExceptionToggle), false),
            TestMockFactory.CreateMockDateTimeService(true)
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
            TestMockFactory.CreateMockConfiguration(false),
            TestMockFactory.CreateMockFeatureManager(nameof(Service1Feature.MockService1PermanentExceptionToggle), false),
            TestMockFactory.CreateMockDateTimeService(true)
        );

        service.Run(false);

        mockLogger.VerifyLogDebug($"$input = {Boolean.FalseString}");
    }

    [TestMethod]
    public void Service1ReturnsFalseFromFalseInput()
    {
        var service = TestMockFactory.CreateServiceWithMockDependencies();

        service.Run(false).Should().BeFalse();
    }

    [TestMethod]
    public void Service1ReturnsTrueFromTrueInput()
    {
        var service = TestMockFactory.CreateServiceWithMockDependencies();

        service.Run(true).Should().BeTrue();
    }

    [TestMethod]
    public void Service1HealthCheckReturnsHealthy()
    {
        var registration = new HealthCheckRegistration(
            name: "MockHealthCheck",
            instance: TestMockFactory.CreateMockHealthCheck(),
            failureStatus: null,
            tags: new[] { "tags" }
        );

        var context = new HealthCheckContext
        {
            Registration = registration
        };

        var service = new Service1HealthCheck(
            TestMockFactory.CreateMockLogger(),
            TestMockFactory.CreateMockConfiguration(false),
            TestMockFactory.CreateMockFeatureManager(string.Empty, false),
            TestMockFactory.CreateMockDateTimeService(false)
        );

        var result = service.CheckHealthAsync(context).Result;

        result.Status.Should<HealthStatus>().Be(HealthStatus.Healthy);
    }

    [TestMethod]
    public void Service1HealthCheckReturnsUnhealthyWhenMockService1PermanentExceptionToggleIsTrue()
    {
        var registration = new HealthCheckRegistration(
            name: "MockHealthCheck",
            instance: TestMockFactory.CreateMockHealthCheck(),
            failureStatus: null,
            tags: new[] { "tags" }
        );

        var context = new HealthCheckContext
        {
            Registration = registration
        };

        var service = new Service1HealthCheck(
            TestMockFactory.CreateMockLogger(),
            TestMockFactory.CreateMockConfiguration(true),
            TestMockFactory.CreateMockFeatureManager(nameof(Service1Feature.MockService1PermanentExceptionToggle), true),
            TestMockFactory.CreateMockDateTimeService(true)
        );

        var result = service.CheckHealthAsync(context).Result;

        result.Status.Should<HealthStatus>().Be(HealthStatus.Unhealthy);
    }

    [TestMethod]
    public void Service1HealthCheckReturnsHealthyWhenMockService1TransientExceptionToggleIsTrueOnEvenTicks()
    {
        var registration = new HealthCheckRegistration(
            name: "MockHealthCheck",
            instance: TestMockFactory.CreateMockHealthCheck(),
            failureStatus: null,
            tags: new[] { "tags" }
        );

        var context = new HealthCheckContext
        {
            Registration = registration
        };

        var service = new Service1HealthCheck(
            TestMockFactory.CreateMockLogger(),
            TestMockFactory.CreateMockConfiguration(false),
            TestMockFactory.CreateMockFeatureManager(nameof(Service1Feature.MockService1TransientExceptionToggle), true),
            TestMockFactory.CreateMockDateTimeService(true) // even ticks
        );

        var result = service.CheckHealthAsync(context).Result;

        result.Status.Should<HealthStatus>().Be(HealthStatus.Healthy);
    }

    [TestMethod]
    public void Service1HealthCheckReturnsUnhealthyWhenMockService1TransientExceptionToggleIsTrueOnOddTicks()
    {
        var registration = new HealthCheckRegistration(
            name: "MockHealthCheck",
            instance: TestMockFactory.CreateMockHealthCheck(),
            failureStatus: null,
            tags: new[] { "tags" }
        );

        var context = new HealthCheckContext
        {
            Registration = registration
        };

        var service = new Service1HealthCheck(
            TestMockFactory.CreateMockLogger(),
            TestMockFactory.CreateMockConfiguration(false),
            TestMockFactory.CreateMockFeatureManager(nameof(Service1Feature.MockService1TransientExceptionToggle), true),
            TestMockFactory.CreateMockDateTimeService(false) // odd ticks
        );

        var result = service.CheckHealthAsync(context).Result;

        result.Status.Should<HealthStatus>().Be(HealthStatus.Unhealthy);
    }
}
