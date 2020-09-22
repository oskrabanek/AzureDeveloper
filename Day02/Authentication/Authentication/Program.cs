using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace Authentication
{
    class Program
    {
        private const string _tenantId = "DIRECTORY_TENANT_ID";
        private const string _clientId = "APPLICATION_CLIENT_ID";

        public static async Task Main(string[] args)
        {
            var app =
                PublicClientApplicationBuilder
                    .Create(_clientId)
                    .WithAuthority(AzureCloudInstance.AzurePublic, _tenantId)
                    .WithRedirectUri("http://localhost/")
                    .Build();

            string[] scopes = { "user.read" };
            AuthenticationResult result = await app.AcquireTokenInteractive(scopes).ExecuteAsync();
            
            var name = GetName(result.AccessToken);

            Console.WriteLine($"Hello {name}");
        }

        private static string GetName(string token)
        {
            var stream = token;
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(stream);
            var tokenS = handler.ReadToken(stream) as JwtSecurityToken;
            return tokenS.Claims.First(claim => claim.Type == "name").Value;
        }
    }
}
