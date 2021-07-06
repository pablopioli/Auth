using System;
using System.Threading.Tasks;
using IdentityModel.OidcClient;

namespace ConsoleApp
{
    static class AuthorizationCode
    {
        internal static async Task Test()
        {
            var port = 23480;

            var browser = new Browser(port);

            var options = new OidcClientOptions
            {
                Authority = "https://localhost:5001",
                ClientId = "desktopapp",
                ClientSecret = "123456",
                RedirectUri = string.Format($"http://127.0.0.1:{port}"),
                Scope = "api",
                FilterClaims = false,
                Browser = browser,
                LoadProfile = false
            };

            var oidcClient = new OidcClient(options);
            var result = await oidcClient.LoginAsync(new LoginRequest());

            if (result.IsError)
            {
                Console.WriteLine("Not authenticated");
            }
            else
            {
                Console.WriteLine("Access token");
                Console.WriteLine(result.AccessToken);
            }
        }
    }
}
