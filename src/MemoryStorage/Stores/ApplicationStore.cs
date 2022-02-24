using MemoryStorage.DataSource;
using MemoryStorage.Domain;
using OpenIddict.Abstractions;
using System.Collections.Immutable;
using System.Globalization;
using System.Text.Json;
using SR = OpenIddict.Abstractions.OpenIddictResources;

namespace MemoryStorage.Stores;

public class ApplicationStore : IOpenIddictApplicationStore<Application>
{
    private readonly ApplicationDataSource _applicationDataSource;

    public ApplicationStore(ApplicationDataSource applicationDataSource)
    {
        _applicationDataSource = applicationDataSource;
    }

    public ValueTask<long> CountAsync(CancellationToken cancellationToken)
    {
        return ValueTask.FromResult((long)_applicationDataSource.Applications.Count);
    }

    public ValueTask<long> CountAsync<TResult>(Func<IQueryable<Application>, IQueryable<TResult>> query, CancellationToken cancellationToken)
    {
        var count = query(_applicationDataSource.Applications.AsQueryable()).LongCount();
        return ValueTask.FromResult(count);
    }

    public ValueTask CreateAsync(Application application, CancellationToken cancellationToken)
    {
        _applicationDataSource.Add(application);
        return ValueTask.CompletedTask;
    }

    public ValueTask DeleteAsync(Application application, CancellationToken cancellationToken)
    {
        _applicationDataSource.Remove(application);
        return ValueTask.CompletedTask;
    }

    public ValueTask<Application?> FindByClientIdAsync(string identifier, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(identifier))
        {
            throw new ArgumentException(SR.GetResourceString(SR.ID0195), nameof(identifier));
        }

        var app = _applicationDataSource.Applications.FirstOrDefault(x => x.ClientId == identifier);

        return ValueTask.FromResult(app);
    }

    public ValueTask<Application?> FindByIdAsync(string identifier, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(identifier))
        {
            throw new ArgumentException(SR.GetResourceString(SR.ID0195), nameof(identifier));
        }

        var app = _applicationDataSource.Applications.FirstOrDefault(x => x.Id == identifier);

        return ValueTask.FromResult(app);
    }

    public IAsyncEnumerable<Application> FindByPostLogoutRedirectUriAsync(string address, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(address))
        {
            throw new ArgumentException(SR.GetResourceString(SR.ID0143), nameof(address));
        }

        return FindByPostLogoutRedirectUriInternal();

        async IAsyncEnumerable<Application> FindByPostLogoutRedirectUriInternal()
        {
            foreach (var app in _applicationDataSource.Applications.Where(x => x.PostLogoutRedirectUris.Any(y => y == address)))
            {
                yield return await Task.FromResult(app);
            }
        }
    }

    public IAsyncEnumerable<Application> FindByRedirectUriAsync(string address, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(address))
        {
            throw new ArgumentException(SR.GetResourceString(SR.ID0143), nameof(address));
        }

        return FindByRedirectUriInternal();

        async IAsyncEnumerable<Application> FindByRedirectUriInternal()
        {
            foreach (var app in _applicationDataSource.Applications.Where(x => x.RedirectUris.Any(y => y == address)))
            {
                yield return await Task.FromResult(app);
            }
        }
    }

    public ValueTask<TResult> GetAsync<TState, TResult>(Func<IQueryable<Application>, TState, IQueryable<TResult>> query, TState state, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask<string?> GetClientIdAsync(Application application, CancellationToken cancellationToken)
    {
        Check.NotNull(application, nameof(application));
        return new ValueTask<string?>(application.ClientId);
    }

    public ValueTask<string?> GetClientSecretAsync(Application application, CancellationToken cancellationToken)
    {
        Check.NotNull(application, nameof(application));

        return new ValueTask<string?>(application.ClientSecret);
    }

    public ValueTask<string?> GetClientTypeAsync(Application application, CancellationToken cancellationToken)
    {
        Check.NotNull(application, nameof(application));

        return new ValueTask<string?>(application.Type);
    }

    public ValueTask<string?> GetConsentTypeAsync(Application application, CancellationToken cancellationToken)
    {
        Check.NotNull(application, nameof(application));

        return new ValueTask<string?>(application.ConsentType);
    }

    public ValueTask<string?> GetDisplayNameAsync(Application application, CancellationToken cancellationToken)
    {
        Check.NotNull(application, nameof(application));

        return new ValueTask<string?>(application.DisplayName);
    }

    public ValueTask<ImmutableDictionary<CultureInfo, string>> GetDisplayNamesAsync(Application application, CancellationToken cancellationToken)
    {
        Check.NotNull(application, nameof(application));

        if (application.DisplayNames is null || application.DisplayNames.Count == 0)
        {
            return new ValueTask<ImmutableDictionary<CultureInfo, string>>(ImmutableDictionary.Create<CultureInfo, string>());
        }

        return new ValueTask<ImmutableDictionary<CultureInfo, string>>(application.DisplayNames.ToImmutableDictionary());
    }

    public ValueTask<string?> GetIdAsync(Application application, CancellationToken cancellationToken)
    {
        Check.NotNull(application, nameof(application));

        return new ValueTask<string?>(application.Id);
    }

    public ValueTask<ImmutableArray<string>> GetPermissionsAsync(Application application, CancellationToken cancellationToken)
    {
        Check.NotNull(application, nameof(application));

        if (application.Permissions is null || application.Permissions.Count == 0)
        {
            return new ValueTask<ImmutableArray<string>>(ImmutableArray.Create<string>());
        }

        return new ValueTask<ImmutableArray<string>>(application.Permissions.ToImmutableArray());
    }

    public ValueTask<ImmutableArray<string>> GetPostLogoutRedirectUrisAsync(Application application, CancellationToken cancellationToken)
    {
        Check.NotNull(application, nameof(application));

        if (application.PostLogoutRedirectUris is null || application.PostLogoutRedirectUris.Count == 0)
        {
            return new ValueTask<ImmutableArray<string>>(ImmutableArray.Create<string>());
        }

        return new ValueTask<ImmutableArray<string>>(application.PostLogoutRedirectUris.ToImmutableArray());
    }

    public ValueTask<ImmutableDictionary<string, JsonElement>> GetPropertiesAsync(Application application, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask<ImmutableArray<string>> GetRedirectUrisAsync(Application application, CancellationToken cancellationToken)
    {
        Check.NotNull(application, nameof(application));

        if (application.RedirectUris is null || application.RedirectUris.Count == 0)
        {
            return new ValueTask<ImmutableArray<string>>(ImmutableArray.Create<string>());
        }

        return new ValueTask<ImmutableArray<string>>(application.RedirectUris.ToImmutableArray());
    }

    public ValueTask<ImmutableArray<string>> GetRequirementsAsync(Application application, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask<Application> InstantiateAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<Application> ListAsync(int? count, int? offset, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<TResult> ListAsync<TState, TResult>(Func<IQueryable<Application>, TState, IQueryable<TResult>> query, TState state, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask SetClientIdAsync(Application application, string? identifier, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask SetClientSecretAsync(Application application, string? secret, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask SetClientTypeAsync(Application application, string? type, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask SetConsentTypeAsync(Application application, string? type, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask SetDisplayNameAsync(Application application, string? name, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask SetDisplayNamesAsync(Application application, ImmutableDictionary<CultureInfo, string> names, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask SetPermissionsAsync(Application application, ImmutableArray<string> permissions, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask SetPostLogoutRedirectUrisAsync(Application application, ImmutableArray<string> addresses, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask SetPropertiesAsync(Application application, ImmutableDictionary<string, JsonElement> properties, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask SetRedirectUrisAsync(Application application, ImmutableArray<string> addresses, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask SetRequirementsAsync(Application application, ImmutableArray<string> requirements, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask UpdateAsync(Application application, CancellationToken cancellationToken)
    {
        throw new InvalidOperationException();
    }
}
