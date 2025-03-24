using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;

namespace MyClassLibrary;

public class MyService(ILogger<MyService> logger, IConfiguration configuration, IFeatureManager featureManager, IMyRepository repository) : IMyService
{
    public async Task<bool> MyMethodAsync(bool myInput)
    {
        logger.LogInformation("Entering {name}", nameof(MyService));

        logger.LogDebug("myInput = {myInput}", myInput);

        string? _myConfiguration;
        try
        {
            logger.LogInformation("Configuring MyConfiguration");
            _myConfiguration = configuration["MyConfiguration"];
            logger.LogDebug("_myConfiguration = {_myConfiguration}", _myConfiguration);
        }
        catch (Exception ex)
        {
            logger.LogError("Error Configuring MyConfiguration Because {message}", ex.Message);
            throw;
        }

        string? _mySecret;
        try
        {
            logger.LogInformation("Configuring MySecret");
            _mySecret = configuration["MySecret"];
            logger.LogDebug("_mySecret = {_mySecret}", _mySecret);
        }
        catch (Exception ex)
        {
            logger.LogError("Error Getting MySecret Because {message}", ex.Message);
            throw;
        }

        bool? _myFeature;
        try
        {
            logger.LogInformation("Configuring MyFeature");
            _myFeature = featureManager.IsEnabledAsync("MyFeature").Result;
            logger.LogDebug("_myFeature = {_myFeature}", _myFeature);
        }
        catch (Exception ex)
        {
            logger.LogError("Error Getting MyFeature Because {message}", ex.Message);
            throw;
        }

        if ((bool)_myFeature)
        {
            logger.LogInformation("Saving Enabled");

            logger.LogInformation("Saving Input To Repository");
            await repository.SaveAsync(myInput);
        }
        else
        {
            logger.LogInformation("Saving Disabled");
        }

        logger.LogInformation("Returning {result}", myInput);

        logger.LogInformation("Exiting {name}", nameof(MyService));

        return await Task.FromResult(myInput);
    }
}
