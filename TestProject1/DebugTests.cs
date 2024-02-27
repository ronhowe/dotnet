/*******************************************************************************
https://github.com/ronhowe
*******************************************************************************/

using Azure.Core;
using Azure.Identity;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;

namespace TestProject1;

[TestClass]
public class DebugTests : TestBase
{
    [TestMethod]
    public async Task POST()
    {
        await Task.Run(() => Console.WriteLine($"POST {DateTime.UtcNow}"));
        //await AuthenticationMethod1();
        //await AuthenticationMethod2();
    }

    [TestMethod]
    [Ignore]
    public async Task AuthenticationMethod1()
    {
        // https://github.com/Azure-Samples/active-directory-dotnetcore-daemon-v2/tree/master/2-Call-OwnApi
        // https://learn.microsoft.com/en-us/dotnet/api/overview/azure/identity-readme?view=azure-dotnet#managed-identity-support

        try
        {
            string tenantId = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";
            string clientId = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";
            string clientSecret = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";
            string authority = $"https://login.microsoftonline.com/{tenantId}";
            string[] scope = [".default"];

            var app = ConfidentialClientApplicationBuilder.Create(clientId)
                .WithAuthority(authority)
                .WithClientSecret(clientSecret)
                .Build();

            var result = await app.AcquireTokenForClient(scope)
                .ExecuteAsync();

            Console.WriteLine($"Access Token: {result.AccessToken}");

            try
            {
                var handler = new JsonWebTokenHandler();

                Console.WriteLine("Claims in Bearer Token:");

                if (handler.ReadToken(result.AccessToken) is JsonWebToken token && token.Claims.Any())
                {
                    foreach (var claim in token.Claims)
                    {
                        Console.WriteLine($"{claim.Type}: {claim.Value}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Reading Bearer Token: {ex.Message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    [TestMethod]
    [Ignore]
    public async Task AuthenticationMethod2()
    {
        try
        {
            var tokenCredential = new DefaultAzureCredential();
            var accessToken = await tokenCredential.GetTokenAsync(
                new TokenRequestContext(scopes: ["https://management.azure.com/.default"])
            );

            Console.WriteLine($"Access Token: {accessToken}");

            try
            {
                var handler = new JsonWebTokenHandler();

                Console.WriteLine("Claims in Bearer Token:");

                if (handler.ReadToken(accessToken.Token) is JsonWebToken token && token.Claims.Any())
                {
                    foreach (var claim in token.Claims)
                    {
                        Console.WriteLine($"{claim.Type}: {claim.Value}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Reading Bearer Token: {ex.Message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}