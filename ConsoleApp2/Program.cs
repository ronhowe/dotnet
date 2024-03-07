/*******************************************************************************
https://github.com/ronhowe
*******************************************************************************/

using Azure.Core;
using Azure.Identity;
using Microsoft.IdentityModel.JsonWebTokens;

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