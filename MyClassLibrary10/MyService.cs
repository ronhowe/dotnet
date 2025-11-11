using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;

namespace MyClassLibrary10;

public class MyService(ILogger<MyService> logger, IConfiguration configuration, IFeatureManager featureManager, IMyRepository repository) : IMyService
{
    public async Task<bool> MyMethodAsync(bool myInput)
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Entering {name}", nameof(MyService));
        }

        if (logger.IsEnabled(LogLevel.Debug))
        {
            logger.LogDebug("myInput = {myInput}", myInput);
        }

        string? _myConfiguration;
        try
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Configuring MyConfiguration");
            }

            _myConfiguration = configuration["MyConfiguration"];

            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug("_myConfiguration = {_myConfiguration}", _myConfiguration);
            }
        }
        catch (Exception ex)
        {
            if (logger.IsEnabled(LogLevel.Error))
            {
                logger.LogError("Error Configuring MyConfiguration Because {message}", ex.Message);
            }

            throw;
        }

        string? _mySecret;

        try
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Configuring MySecret");
            }

            _mySecret = configuration["MySecret"];

            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug("_mySecret = {_mySecret}", _mySecret);
            }
        }
        catch (Exception ex)
        {
            if (logger.IsEnabled(LogLevel.Error))
            {
                logger.LogError("Error Getting MySecret Because {message}", ex.Message);
            }

            throw;
        }

        bool? _myFeature;

        try
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Configuring MyFeature");
            }

            _myFeature = featureManager.IsEnabledAsync("MyFeature").Result;

            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug("_myFeature = {_myFeature}", _myFeature);
            }
        }
        catch (Exception ex)
        {
            if (logger.IsEnabled(LogLevel.Error))
            {
                logger.LogError("Error Getting MyFeature Because {message}", ex.Message);
            }

            throw;
        }

        if ((bool)_myFeature)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Saving Enabled");
            }

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Saving Input To Repository");
            }

            await repository.SaveAsync(myInput);
        }
        else
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Saving Disabled");
            }
        }

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Returning {result}", myInput);
        }

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Exiting {name}", nameof(MyService));
        }

        return await Task.FromResult(myInput);
    }
}
