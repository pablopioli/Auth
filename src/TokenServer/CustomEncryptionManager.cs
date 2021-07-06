using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using OpenIddict.MemoryStorage.Domain;

namespace TokenServer
{
    public class CustomEncryptionManager : OpenIddictApplicationManager<Application>
    {
        public CustomEncryptionManager(
            IOpenIddictApplicationCache<Application> cache,
            ILogger<OpenIddictApplicationManager<Application>> logger,
            IOptionsMonitor<OpenIddictCoreOptions> options,
            IOpenIddictApplicationStoreResolver resolver)
            : base(cache, logger, options, resolver)
        { }

        protected override ValueTask<bool> ValidateClientSecretAsync(string secret, string comparand, CancellationToken cancellationToken = default)
        {
            // We use non encrypted values for testing
            // Don't do this on production
            return ValueTask.FromResult(secret == comparand);
        }
    }
}
