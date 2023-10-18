/*******************************************************************************
https://github.com/ronhowe
*******************************************************************************/

using ClassLibrary1;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Moq;

namespace TestProject1;

internal static class TestMockFactory
{

    internal static IConfiguration CreateMockConfiguration(bool value)
    {
        //link - https://adamstorr.azurewebsites.net/blog/mocking-ilogger-with-moq
        var mockConfigurationSection = new Mock<IConfigurationSection>();
        mockConfigurationSection.Setup(x => x.Value).Returns(value.ToString());

        var mockConfiguration = new Mock<IConfiguration>();
        mockConfiguration.Setup(x => x.GetSection(nameof(Service1Feature.MockService1PermanentExceptionToggle))).Returns(mockConfigurationSection.Object);
        mockConfiguration.Setup(x => x.GetSection(nameof(Service1Feature.MockService1TransientExceptionToggle))).Returns(mockConfigurationSection.Object);

        return mockConfiguration.Object;
    }

    internal static IDateTimeService CreateMockDateTimeService(bool even)
    {
        long ticks = even ? (DateTime.UtcNow.Ticks / 2) * 2 : ((DateTime.UtcNow.Ticks / 2) * 2) + 1;
        DateTime dateTime = new(ticks);

        var mockDateTimeService = new Mock<IDateTimeService>();
        mockDateTimeService.Setup(x => x.UtcNow).Returns(dateTime);

        return mockDateTimeService.Object;
    }

    internal static IFeatureManager CreateMockFeatureManager(string name, bool value)
    {
        var mockFeatureManager = new Mock<IFeatureManager>();
        mockFeatureManager.Setup(x => x.IsEnabledAsync(name).Result).Returns(value);

        return mockFeatureManager.Object;
    }

    internal static IHealthCheck CreateMockHealthCheck()
    {
        var mockHealthCheck = new Mock<IHealthCheck>();

        return mockHealthCheck.Object;
    }

    internal static ILogger<Service1> CreateMockLogger()
    {
        var mockLogger = new Mock<ILogger<Service1>>();

        return mockLogger.Object;
    }

    internal static Service1 CreateServiceWithMockDependencies()
    {
        var service = new Service1(
            CreateMockLogger(),
            CreateMockConfiguration(false),
            CreateMockFeatureManager(string.Empty, false),
            CreateMockDateTimeService(false)
        );

        return service;
    }
}