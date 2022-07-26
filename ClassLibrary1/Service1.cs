﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;

namespace ClassLibrary1;

public class Service1 : IService1
{
    private readonly ILogger<Service1> _logger;
    private readonly IConfiguration _config;
    private readonly IFeatureManager _featureManager;

    public Service1(ILogger<Service1> logger, IConfiguration config, IFeatureManager featureManager)
    {
        _logger = logger;
        _config = config;
        _featureManager = featureManager;
    }

    public bool Run(bool? input)
    {
        _logger.LogInformation("Running Service");

        _logger.LogDebug("input={input}", input);

        //help - example of reading boolean from config via iconfiguration
        var config = _config.GetSection(nameof(Service1Feature.MockService1ExceptionToggle)).Value;
        _logger.LogDebug("config={config}", config);

        //help - example of reading boolean from config via ifeaturemanager
        var feature = _featureManager.IsEnabledAsync(nameof(Service1Feature.MockService1ExceptionToggle)).Result;
        _logger.LogDebug("feature={feature}", feature);

        if (feature)
        {
            _logger.LogWarning("Throwing MockServiceException");
            throw new MockService1Exception(nameof(Service1Feature.MockService1ExceptionToggle));
        }

        //todo - mock resource throttling
        /*
        void MockCpuThrottle()
        {
            int iterations = Int32.Parse(_configuration["iterations"]);

            logger.LogDebug("iterations = " + iterations);

            if (iterations > 20000)
            {
                iterations = 20000;
            }

            List<int> primes = new List<int>();

            bool isPrime = true;

            for (int i = 2; i <= iterations; i++)
            {
                for (int j = 2; j <= iterations; j++)
                {
                    if (i != j && i % j == 0)
                    {
                        isPrime = false;
                        break;
                    }
                }

                if (isPrime)
                {
                    primes.Add(i);
                }

                isPrime = true;
            }
        }
       */

        return input != null && input.Value;
    }
}

public interface IService1
{
    public bool Run(bool? input);
}
