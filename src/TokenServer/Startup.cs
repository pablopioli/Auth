using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using OpenIddict.MemoryStorage;
using OpenIddict.MemoryStorage.DataSource;
using OpenIddict.MemoryStorage.Domain;
using OpenIddict.MemoryStorage.Stores;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace TokenServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
            => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddOpenIddict()

                .AddCore(options =>
                {
                    // Don't do this on production
                    // This is to check the produced tokens with a JWT decryption tool
                    options.ReplaceApplicationManager(typeof(CustomEncryptionManager));

                    // Read the apps from a Json file or inject a sample one
                    var apps = new ApplicationDataSource();
                    var preconfiguredApps = Configuration.GetValue<string>("APPLICATIONS");
                    if (!string.IsNullOrEmpty(preconfiguredApps))
                    {
                        var json = File.ReadAllText(preconfiguredApps);
                        foreach (var app in JsonConvert.DeserializeObject<List<Application>>(json))
                        {
                            apps.Add(app);
                        }
                    }
                    else
                    {
                        apps.Add(new Application("console")
                        {
                            ClientId = "console",
                            ClientSecret = "123456",
                            Permissions = new[] {
                                      Permissions.Endpoints.Token,
                                      Permissions.GrantTypes.ClientCredentials,
                                      Permissions.GrantTypes.RefreshToken,
                                      Permissions.Prefixes.Scope + "api",
                        }
                        });
                    }
                    services.AddSingleton(apps);

                    // Read the scopes from a Json file or inject a sample one
                    var scopes = new ScopeDataSource();
                    var preconfiguredScopes = Configuration.GetValue<string>("SCOPES");
                    if (!string.IsNullOrEmpty(preconfiguredScopes))
                    {
                        var json = File.ReadAllText(preconfiguredScopes);
                        foreach (var scope in JsonConvert.DeserializeObject<List<Scope>>(json))
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
                    services.AddSingleton(scopes);

                    // Use in memory storage
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
                    services.AddSingleton(storageOptions);
                })

                .AddServer(options =>
                {
                    // Don't do this on production
                    options.DisableAccessTokenEncryption();

                    options.SetTokenEndpointUris("/connect/token");

                    options.AllowClientCredentialsFlow();
                    options.AllowRefreshTokenFlow();

                    options.AddDevelopmentEncryptionCertificate()
                           .AddDevelopmentSigningCertificate();

                    options.UseAspNetCore()
                           .EnableTokenEndpointPassthrough();
                });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(options =>
            {
                options.MapControllers();
                options.MapDefaultControllerRoute();
            });

            app.UseWelcomePage();
        }
    }
}
