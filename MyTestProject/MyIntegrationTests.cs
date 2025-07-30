using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyClassLibrary;
using Serilog;
using Shouldly;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace MyTestProject;

[TestClass]
public sealed class MyIntegrationTests : TestBase
{
    [TestMethod]
    [TestCategory("IntegrationTest")]
    [DataRow(false)]
    [DataRow(true)]
    public async Task MyDebugHostTests(bool value)
    {
        Debug.WriteLine($"Creating Service Collection");
        ServiceCollection _serviceCollection = new();

        Debug.WriteLine($"Adding Logging");
        _serviceCollection.AddLogging(configure =>
        {
            Debug.WriteLine($"Clearing Log Providers");
            configure.ClearProviders();

            Debug.WriteLine($"Adding Serilog");
            configure.AddSerilog();

            LogLevel _logLevel = LogLevel.Trace;
            Debug.WriteLine($"Setting Minimum Log Level = {_logLevel}");
            configure.SetMinimumLevel(_logLevel);
        });

        Debug.WriteLine($"Adding Configuration");
        Dictionary<string, string?> _configurationSettings = new()
        {
            { "ConnectionStrings:MyAzureStorage", "UseDevelopmentStorage=true;" },
            { "ConnectionStrings:MyDatabase", "Server=localhost;Database=MyDatabase;Integrated Security=True;Application Name=MyTestProject;Encrypt=False;Connect Timeout=1;Command Timeout=0;" },
            { "FeatureManagement:MyFeature", "false" },
            { "MyConfiguration", "MyTestProject" },
            { "MyHeader", "MyHeader" },
            { "MySecret", "MyTestProject" }
        };
        IConfigurationRoot _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(_configurationSettings)
            .Build();
        _serviceCollection.AddSingleton<IConfiguration>(_configuration);

        Debug.WriteLine($"Adding Feature Management");
        _serviceCollection.AddFeatureManagement();

        Debug.WriteLine($"Adding {nameof(MyRepository)}");
        _serviceCollection.AddTransient<IMyRepository, MyRepository>();

        Debug.WriteLine($"Adding {nameof(MyService)}");
        _serviceCollection.AddTransient<MyService>();

        Debug.WriteLine($"Adding Host Environment");
        _serviceCollection.AddSingleton<IHostEnvironment>(new HostingEnvironment
        {
            EnvironmentName = Environments.Development
        });

        Debug.WriteLine($"Building Service Provider");
        ServiceProvider _serviceProvider = _serviceCollection.BuildServiceProvider();

        Debug.WriteLine($"Getting {nameof(MyService)}");
        MyService? _myService = _serviceProvider.GetService<MyService>();

        if (_myService is not null)
        {
            Debug.WriteLine($"Calling {nameof(MyService)} With {value}");
            bool _result = await _myService.MyMethodAsync(value);

            Debug.WriteLine($"Asserting Result Is {value}");
            _result.ShouldBe(value);
        }
        else
        {
            Debug.WriteLine($"{nameof(MyService)} Not Found In Service Collection");
            Assert.Inconclusive("TEST FAILED");
        }
    }

    [TestMethod]
    [TestCategory("IntegrationTest")]
    // TODO: Production fails unless runtime security context has Azure.Identity based permissions.
    //[DataRow(false, "Production", "1")]
    //[DataRow(false, "Production", "2")]
    //[DataRow(true, "Production", "1")]
    //[DataRow(true, "Production", "2")]
    [DataRow(false, "Development", "1")]
    [DataRow(false, "Development", "2")]
    [DataRow(true, "Development", "1")]
    [DataRow(true, "Development", "2")]
    public async Task MyWebHostTests(bool value, string environmentName, string version)
    {
        Debug.WriteLine($"Building Web Host");
        using WebApplicationFactory<Program> _application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment(environmentName);
            builder.ConfigureKestrel(serverOptions =>
            {
                serverOptions.ListenLocalhost(5001, listenOptions =>
                {
                    listenOptions.UseHttps();
                });
            });
        });

        Debug.WriteLine($"Creating Client");
        using HttpClient _client = _application.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost:5001")
        });

        Debug.WriteLine($"Generating Bearer Token");
        JwtSecurityTokenHandler _tokenHandler = new();
        byte[] key = Encoding.UTF8.GetBytes($"/{new string('*', 4096 / 8)}");
        SecurityTokenDescriptor _tokenDescriptor = new()
        {
            Subject = new ClaimsIdentity([new Claim("MyClaimType", "MyClaimValue")]),
            Expires = DateTime.UtcNow.AddMinutes(30),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = "yourIssuer",
            Audience = "yourAudience"
        };
        SecurityToken _token = _tokenHandler.CreateToken(_tokenDescriptor);
        string _tokenString = _tokenHandler.WriteToken(_token);
        Debug.WriteLine(_tokenString);

        Debug.WriteLine($"Sending GET Request With {value}");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenString);
        using HttpResponseMessage _response = await _client.GetAsync($"/v{version}/{nameof(MyService)}?input={value}");

        Debug.WriteLine($"Asserting HTTP Status Code Is {HttpStatusCode.OK}");
        _response.StatusCode.ShouldBe(HttpStatusCode.OK);

        foreach (KeyValuePair<string, IEnumerable<string>> header in _response.Headers)
        {
            Debug.WriteLine($"{header.Key}: {string.Join(", ", header.Value)}");
        }

        Debug.WriteLine($"Asserting Header");
        if (_response.Headers.TryGetValues("MyHeader", out var values))
        {
            values.First().ShouldBe($"MyHeader ({environmentName})");
        }

        Debug.WriteLine($"Asserting API Supported Versions Header");
        if (_response.Headers.TryGetValues("api-supported-versions", out var values2))
        {
            values2.First().ShouldBe("1, 2");
        }

        Debug.WriteLine($"Asserting Result Is {value}");
        Boolean.Parse(_response.Content.ReadAsStringAsync().Result).ShouldBe(value);
    }
}
