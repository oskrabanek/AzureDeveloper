using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Microsoft.Graph;
using Microsoft.Graph.Auth;

namespace Authentication
{
    class Program
    {
        private const string _tenantId = "DIRECTORY_TENANT_ID";
        private const string _clientId = "APPLICATION_CLIENT_ID";

        private static string[] _scopes = { "user.read" };

        public static async Task Main(string[] args)
        {
            var app =
                PublicClientApplicationBuilder
                    .Create(_clientId)
                    .WithAuthority(AzureCloudInstance.AzurePublic, _tenantId)
                    .WithRedirectUri("http://localhost/")
                    .Build();

            await GetNameFromMSAL(app);

            //await GetNameFromGraph(app);
        }

        private static async Task GetNameFromMSAL(IPublicClientApplication app)
        {
            AuthenticationResult result = await app.AcquireTokenInteractive(_scopes).ExecuteAsync();

            var stream = result.AccessToken;
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(stream);
            var tokenS = handler.ReadToken(stream) as JwtSecurityToken;
            var name = tokenS.Claims.First(claim => claim.Type == "name").Value;

            Console.WriteLine($"MSAL: Hello {name}");
        }

        private static async Task GetNameFromGraph(IPublicClientApplication app)
        {
            var provider = new InteractiveAuthenticationProvider(app, _scopes);
            var client = new GraphServiceClient(provider);

            User me = await client.Me.Request().GetAsync();
            Console.WriteLine($"Graph: Hello {me.DisplayName}");
        }
    }
}
