using Azure.Data.Tables;
using Azure.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;

namespace MyClassLibrary10;

public class MyRepository(ILogger<MyRepository> logger, IConfiguration configuration, IHostEnvironment environment) : IMyRepository
{
    public async Task SaveAsync(bool myInput)
    {
        logger.LogInformation("Entering {name}", nameof(MyRepository));

        logger.LogDebug("myInput = {myInput}", myInput);

        const string _dbConnection = "MyDatabase";
        string _dbConnectionString;
        try
        {
            logger.LogInformation("Configuring Database Connection String");
            _dbConnectionString = configuration[$"ConnectionStrings:{_dbConnection}"] ?? throw new InvalidOperationException("Database Connection String Not Configured");
            logger.LogDebug("_dbConnectionString = {_dbConnectionString}", _dbConnectionString);
        }
        catch (Exception ex)
        {
            logger.LogError("Error Configuring Database Connection String Because {message}", ex.Message);
            throw;
        }

        const string _azConnection = "MyAzureStorage";
        string _azConnectionString;
        try
        {
            logger.LogInformation("Configuring Azure Storage Connection String");
            _azConnectionString = configuration[$"ConnectionStrings:{_azConnection}"] ?? throw new InvalidOperationException("Azure Storage Connection String Not Configured");
            logger.LogDebug("_azConnectionString = {_azConnectionString}", _azConnectionString);
        }
        catch (Exception ex)
        {
            logger.LogError("Error Configuring Azure Storage Connection String Because {message}", ex.Message);
            throw;
        }

        logger.LogInformation("Generating Row Key");
        string _rowKey = Guid.CreateVersion7().ToString();

        logger.LogInformation("Saving {_rowKey}", _rowKey);

        // TODO: Read retry settings from configuration.
        const int _maxRetries = 2;
        const int _retryMilliseconds = 1;

        logger.LogInformation("Creating Retry Policy");
        var _retryPolicy = Policy
            .Handle<SqlException>()
            .WaitAndRetryAsync(_maxRetries, retryAttempt => TimeSpan.FromMilliseconds(_retryMilliseconds),
                (ex, timeSpan, retryAttempt, context) =>
                {
                    logger.LogError("Save Failed Because {message}", ex.Message);
                    logger.LogWarning("Retry Attempt # {retryAttempt} Of {maxRetries}", retryAttempt, _maxRetries);
                });

        bool _dbSaved = false;
        bool _azSaved = false;

        try
        {
            logger.LogDebug("Executing With Retry Policy");
            await _retryPolicy.ExecuteAsync(async () =>
            {
                if (!_dbSaved)
                {
                    logger.LogInformation("Saving To Database");

                    logger.LogInformation("Opening Connection");
                    using SqlConnection _sqlConnection = new(_dbConnectionString);
                    await _sqlConnection.OpenAsync();

                    logger.LogInformation("Executing Command");
                    using SqlCommand _sqlCommand = new("INSERT [dbo].[MyTable] ([PartitionKey], [RowKey], [MyInput]) VALUES (@PartitionKey, @RowKey, @MyInput);", _sqlConnection);
                    _sqlCommand.Parameters.AddWithValue("@PartitionKey", DateTime.UtcNow);
                    _sqlCommand.Parameters.AddWithValue("@RowKey", _rowKey);
                    _sqlCommand.Parameters.AddWithValue("@MyInput", myInput);
                    await _sqlCommand.ExecuteNonQueryAsync();
                }

                _dbSaved = true;
                logger.LogInformation("Save To Database Succeeded");

                if (!_azSaved)
                {
                    logger.LogInformation("Saving To Azure Storage");

                    logger.LogInformation("Creating Table");
                    TableClient _tableClient;
                    if (environment.IsDevelopment())
                    {
                        _tableClient = new(_azConnectionString, "MyCloudTable");
                    }
                    else
                    {
                        var _credential = new DefaultAzureCredential();
                        _tableClient = new TableClient(new Uri(_azConnectionString), "MyCloudTable", _credential);
                    }
                    await _tableClient.CreateIfNotExistsAsync();

                    logger.LogInformation("Adding Entity");
                    TableEntity _tableEntity = new(DateTime.UtcNow.ToString("yyyy-MM-dd"), _rowKey)
                    {
                        { "MyInput", myInput }
                    };
                    await _tableClient.AddEntityAsync(_tableEntity);
                }

                _azSaved = true;
                logger.LogInformation("Save To Azure Storage Succeeded");
            });
        }
        catch (Exception ex)
        {
            logger.LogError("Save Failed Because {message}", ex.Message);
            logger.LogCritical("CRITICAL ERROR");
            throw;
        }

        logger.LogInformation("Exiting {name}", nameof(MyRepository));
    }
}
