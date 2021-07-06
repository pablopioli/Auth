using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;

namespace ConsoleApp
{
    static class ClientCredentials
    {
        internal static async Task Test()
        {
            using var client = new HttpClient();

            (string token, string refreshToken) = await GetTokenAsync(client);
            Console.WriteLine("Access token: {0}", token);
            Console.WriteLine();
            Console.WriteLine("Refresh token: {0}", refreshToken);
            Console.WriteLine();

            await ValidateToken(token, client);

            (token, refreshToken) = await UseRefreshToken(refreshToken, client);
            Console.WriteLine("Access token: {0}", token);
            Console.WriteLine();
            Console.WriteLine("Refresh token: {0}", refreshToken);
            Console.WriteLine();
        }

        private static async Task<(string, string)> GetTokenAsync(HttpClient client)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:5001/connect/token")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["grant_type"] = "client_credentials",
                    ["client_id"] = "clientcredentials",
                    ["client_secret"] = "123456",
                    ["scope"] = "offline_access api"
                })
            };

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead);

            var payload = await response.Content.ReadFromJsonAsync<OpenIddictResponse>();

            if (!string.IsNullOrEmpty(payload.Error))
            {
                throw new InvalidOperationException("An error occurred while retrieving an access token.");
            }

            return (payload.AccessToken, payload.RefreshToken);
        }

        private static async Task ValidateToken(string token, HttpClient client)
        {
            var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5001");

            var keys = new List<SecurityKey>();
            foreach (var webKey in disco.KeySet.Keys)
            {
                var e = Base64Url.Decode(webKey.E);
                var n = Base64Url.Decode(webKey.N);

                var key = new RsaSecurityKey(new RSAParameters { Exponent = e, Modulus = n })
                {
                    KeyId = webKey.Kid
                };

                keys.Add(key);
            }

            var validationParameters = new TokenValidationParameters
            {
                ValidIssuer = "https://localhost:5001/",
                RequireExpirationTime = true,
                RequireSignedTokens = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = keys,
                ValidateLifetime = true,
                ValidateAudience = false,
                ClockSkew = TimeSpan.FromMinutes(2)
            };

            var handler = new JsonWebTokenHandler();
            var validationResult = handler.ValidateToken(token, validationParameters);

            if (validationResult.IsValid)
            {
                Console.WriteLine("Token is valid");
            }
            else
            {
                Console.WriteLine("Token is invalid!");
            }
        }

        private static async Task<(string, string)> UseRefreshToken(string refreshToken, HttpClient client)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:5001/connect/token")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["grant_type"] = "refresh_token",
                    ["client_id"] = "clientcredentials",
                    ["client_secret"] = "123456",
                    ["refresh_token"] = refreshToken
                })
            };

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead);

            var payload = await response.Content.ReadFromJsonAsync<OpenIddictResponse>();

            if (!string.IsNullOrEmpty(payload.Error))
            {
                throw new InvalidOperationException("An error occurred while retrieving an access token.");
            }

            return (payload.AccessToken, payload.RefreshToken);
        }
    }
}
