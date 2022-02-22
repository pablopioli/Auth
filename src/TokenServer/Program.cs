using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using OpenIddict.Abstractions;
using OpenIddict.MemoryStorage;
using OpenIddict.MemoryStorage.DataSource;
using OpenIddict.MemoryStorage.Domain;
using OpenIddict.MemoryStorage.Stores;
using System.Text.Json;
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
           // Don't do this on production
           // This is to check the produced tokens with a JWT decryption tool
           if (builder.Environment.IsDevelopment())
           {
               options.ReplaceApplicationManager(typeof(CustomEncryptionManager));
           }

           // Read the apps from a Json file or inject a sample one
           var apps = new ApplicationDataSource();
           var preconfiguredApps = new List<Application>();

           if (File.Exists("applications.json"))
           {
               preconfiguredApps = JsonSerializer.Deserialize<List<Application>>(File.ReadAllText("applications.json"));
           }

           if (preconfiguredApps.Count != 0)
           {
               foreach (var app in preconfiguredApps)
               {
                   apps.Add(app);
               }
           }
           else
           {
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
           var storageOptions = new OpenIddictMemoryStorageOptions
           {
               AuthorizationFileStorage = "./auth.json",
               TokenFileStorage = "./tokens.json"
           };
           builder.Services.AddSingleton(storageOptions);
       })

    .AddServer(options =>
    {
        // Don't do this on production
        options.DisableAccessTokenEncryption();

        options.SetTokenEndpointUris("/connect/token");
        options.SetAuthorizationEndpointUris("/connect/authorize");
        options.SetUserinfoEndpointUris("/connect/userinfo");

        options.AllowClientCredentialsFlow();
        options.AllowRefreshTokenFlow();
        options.AllowAuthorizationCodeFlow().RequireProofKeyForCodeExchange();

        options.AddDevelopmentEncryptionCertificate()
               .AddDevelopmentSigningCertificate();

        options.UseAspNetCore()
               .EnableTokenEndpointPassthrough()
               .EnableAuthorizationEndpointPassthrough();
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

app.UseWelcomePage();

app.Run();
