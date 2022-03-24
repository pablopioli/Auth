using MemoryStorage;
using MemoryStorage.DataSource;
using MemoryStorage.Domain;
using MemoryStorage.Stores;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using OpenIddict.Abstractions;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using TestServer;
using TokenServer;
using static OpenIddict.Abstractions.OpenIddictConstants;

var builder = WebApplication.CreateBuilder();
builder.Configuration.AddJsonFile("settings.json", true).AddEnvironmentVariables().AddCommandLine(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options => options.LoginPath = "/account/login")
        .AddGitHub("GitHub", options =>
        {
            options.ClientId = builder.Configuration.GetValue<string>("github:clientid");
            options.ClientSecret = builder.Configuration.GetValue<string>("github:clientsecret");

            options.Scope.Add("user:email");

            options.Events = new OAuthEvents
            {
                OnCreatingTicket = _ => Task.CompletedTask
            };
        });

builder.Services.AddOpenIddict()
       .AddCore(options =>
       {
           if (builder.Environment.IsDevelopment())
           {
               // This is to check the produced tokens with a JWT decryption tool
               options.ReplaceApplicationManager(typeof(CustomEncryptionManager));
           }

           // Read the apps from a Json file or inject a sample one
           ApplicationDataSource apps;
           if (File.Exists("applications.json"))
           {
               apps = ApplicationDataSource.FromFile("applications.json");
           }
           else
           {
               apps = new ApplicationDataSource();

               apps.Add(new Application("console")
               {
                   ClientId = "clientcredentials",
                   ClientSecret = "123456",
                   Permissions = new[] {
                                      Permissions.Endpoints.Token,
                                      Permissions.GrantTypes.ClientCredentials,
                                      Permissions.GrantTypes.RefreshToken,
                                      Permissions.Prefixes.Scope + "api",
                                     }
               });

               apps.Add(new Application("desktopapp")
               {
                   ClientId = "desktopapp",
                   ClientSecret = "123456",
                   ConsentType = "Explicit",
                   RedirectUris = new List<string> { "http://127.0.0.1:23480" },
                   Permissions = new[] {
                                      Permissions.Endpoints.Token,
                                      Permissions.Endpoints.Authorization,
                                      Permissions.GrantTypes.ClientCredentials,
                                      Permissions.GrantTypes.AuthorizationCode,
                                      Permissions.GrantTypes.RefreshToken,
                                      Permissions.Prefixes.Scope + "api",
                                      Permissions.ResponseTypes.Code
                },
                   Requirements = new[] {
                                      Requirements.Features.ProofKeyForCodeExchange
                                   }
               });

               apps.Add(new Application("nativeclient")
               {
                   ClientId = "nativeclient",
                   RedirectUris = new List<string> { "http://127.0.0.1:16101" },
                   Type = OpenIddictConstants.ClientTypes.Public,
                   Permissions = new[] {
                                      Permissions.Endpoints.Token,
                                      Permissions.Endpoints.Authorization,
                                      Permissions.GrantTypes.AuthorizationCode,
                                      Permissions.GrantTypes.RefreshToken,
                                      Permissions.Prefixes.Scope + "api",
                                      Permissions.ResponseTypes.Code
                }
               });

               apps.Add(new Application("webapp")
               {
                   ClientId = "webapp",
                   ClientSecret = "123456",
                   RedirectUris = new List<string> { "https://localhost:44901/signin-oidc" },
                   Permissions = new[] {
                                      Permissions.Endpoints.Token,
                                      Permissions.Endpoints.Authorization,
                                      Permissions.GrantTypes.ClientCredentials,
                                      Permissions.GrantTypes.AuthorizationCode,
                                      Permissions.GrantTypes.RefreshToken,
                                      Permissions.Prefixes.Scope + "api",
                                      Permissions.ResponseTypes.Code
},
                   Requirements = new[] {
                                      Requirements.Features.ProofKeyForCodeExchange
                 }
               });
           }

           builder.Services.AddSingleton(apps);

           // Read the scopes from a Json file or inject a sample one
           var scopes = new ScopeDataSource();
           var preconfiguredScopes = builder.Configuration.GetValue<string>("SCOPES");
           if (!string.IsNullOrEmpty(preconfiguredScopes))
           {
               var json = File.ReadAllText(preconfiguredScopes);
               foreach (var scope in JsonSerializer.Deserialize<List<Scope>>(json))
               {
                   scopes.Add(scope);
               }
           }
           else
           {
               scopes.Add(new Scope("api")
               {
                   Name = "api"
               });
           }
           builder.Services.AddSingleton(scopes);

           // Use in-memory storage
           options.SetDefaultApplicationEntity<Application>();
           options.AddApplicationStore<ApplicationStore>();

           options.SetDefaultScopeEntity<Scope>();
           options.AddScopeStore<ScopeStore>();

           options.SetDefaultAuthorizationEntity<Authorization>();
           options.AddAuthorizationStore<AuthorizationStore>();

           options.SetDefaultTokenEntity<Token>();
           options.AddTokenStore<TokenStore>();

           // Save authorizations and tokens in json files
           builder.Services
                  .AddOptions<OpenIddictMemoryStorageOptions>()
                  .Configure(opt =>
                     {
                         opt.AuthorizationFileStorage = "./auth.json";
                         opt.TokenFileStorage = "./tokens.json";
                     });
       })

    .AddServer(options =>
    {
        // Generate a JWT you can look into
        options.DisableAccessTokenEncryption();

        options.SetTokenEndpointUris("/connect/token");
        options.SetAuthorizationEndpointUris("/connect/authorize");
        options.SetRevocationEndpointUris("/connect/revocation");
        options.SetLogoutEndpointUris("/connect/logout");
        options.SetIntrospectionEndpointUris("/connect/introspect");

        options.AllowClientCredentialsFlow();
        options.AllowRefreshTokenFlow();
        options.AllowAuthorizationCodeFlow().RequireProofKeyForCodeExchange();

        // Use two certificates for signing and encryption
        // You should load them from secure storage
        const string encryptionCert = "enc.pfx";
        if (!File.Exists(encryptionCert))
        {
            File.WriteAllBytes(encryptionCert, CertManager.CreateEncryptionCertificate());
        }

        const string signingCert = "sign.pfx";
        if (!File.Exists(signingCert))
        {
            File.WriteAllBytes(signingCert, CertManager.CreateSigningCertificate());
        }

        options.AddEncryptionCertificate(new X509Certificate2(encryptionCert))
               .AddSigningCertificate(new X509Certificate2(signingCert));

        options.UseAspNetCore()
               .EnableTokenEndpointPassthrough()
               .EnableAuthorizationEndpointPassthrough()
               .EnableLogoutEndpointPassthrough()
               .EnableUserinfoEndpointPassthrough();
    })

    .AddValidation();

var app = builder.Build();

app.UseDeveloperExceptionPage();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(options =>
{
    options.MapControllers();
    options.MapDefaultControllerRoute();
});

app.Run();
