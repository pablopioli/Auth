using OpenIddict.Abstractions;
using OpenIddict.MemoryStorage.DataSource;
using OpenIddict.MemoryStorage.Domain;
using System.Collections.Immutable;
using System.Globalization;
using System.Text.Json;
using SR = OpenIddict.Abstractions.OpenIddictResources;

namespace OpenIddict.MemoryStorage.Stores;

/// <summary>
/// Provides methods allowing to manage the scopes stored in a database.
/// </summary>
public class ScopeStore : IOpenIddictScopeStore<Scope>
{
    private readonly ScopeDataSource _scopeDataSource;

    public ScopeStore(ScopeDataSource scopeDataSource)
    {
        _scopeDataSource = scopeDataSource;
    }

    public ValueTask<long> CountAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask<long> CountAsync<TResult>(Func<IQueryable<Scope>, IQueryable<TResult>> query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask CreateAsync(Scope scope, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask DeleteAsync(Scope scope, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask<Scope?> FindByIdAsync(string identifier, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask<Scope?> FindByNameAsync(string name, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<Scope> FindByNamesAsync(ImmutableArray<string> names, CancellationToken cancellationToken)
    {
        if (names.Any(string.IsNullOrEmpty))
        {
            throw new ArgumentException(SR.GetResourceString(SR.ID0203), nameof(names));
        }

        return FindByNamesInternal();

        async IAsyncEnumerable<Scope> FindByNamesInternal()
        {
            foreach (var scope in _scopeDataSource.Scopes.Where(x => names.Any(y => y == x.Name)))
            {
                yield return await Task.FromResult(scope);
            }
        }
    }

    public IAsyncEnumerable<Scope> FindByResourceAsync(string resource, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask<TResult> GetAsync<TState, TResult>(Func<IQueryable<Scope>, TState, IQueryable<TResult>> query, TState state, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask<string?> GetDescriptionAsync(Scope scope, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask<ImmutableDictionary<CultureInfo, string>> GetDescriptionsAsync(Scope scope, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask<string?> GetDisplayNameAsync(Scope scope, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask<ImmutableDictionary<CultureInfo, string>> GetDisplayNamesAsync(Scope scope, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask<string?> GetIdAsync(Scope scope, CancellationToken cancellationToken)
    {
        Check.NotNull(scope, nameof(scope));

        return new ValueTask<string?>(scope.Id);
    }

    public ValueTask<string?> GetNameAsync(Scope scope, CancellationToken cancellationToken)
    {
        Check.NotNull(scope, nameof(scope));

        return new ValueTask<string?>(scope.Name);
    }

    public ValueTask<ImmutableDictionary<string, JsonElement>> GetPropertiesAsync(Scope scope, CancellationToken cancellationToken)
    {
        Check.NotNull(scope, nameof(scope));

        if (scope.Properties is null)
        {
            return new ValueTask<ImmutableDictionary<string, JsonElement>>(ImmutableDictionary.Create<string, JsonElement>());
        }

        using var document = JsonDocument.Parse(scope.Properties);
        var builder = ImmutableDictionary.CreateBuilder<string, JsonElement>();

        foreach (var property in document.RootElement.EnumerateObject())
        {
            builder[property.Name] = property.Value.Clone();
        }

        return new ValueTask<ImmutableDictionary<string, JsonElement>>(builder.ToImmutable());
    }

    public ValueTask<ImmutableArray<string>> GetResourcesAsync(Scope scope, CancellationToken cancellationToken)
    {
        Check.NotNull(scope, nameof(scope));

        if (scope.Resources is null || scope.Resources.Count == 0)
        {
            return new ValueTask<ImmutableArray<string>>(ImmutableArray.Create<string>());
        }

        return new ValueTask<ImmutableArray<string>>(scope.Resources.ToImmutableArray());
    }

    public ValueTask<Scope> InstantiateAsync(CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(new Scope(Guid.NewGuid().ToString()));
    }

    public IAsyncEnumerable<Scope> ListAsync(int? count, int? offset, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<TResult> ListAsync<TState, TResult>(Func<IQueryable<Scope>, TState, IQueryable<TResult>> query, TState state, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask SetDescriptionAsync(Scope scope, string? description, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask SetDescriptionsAsync(Scope scope, ImmutableDictionary<CultureInfo, string> descriptions, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask SetDisplayNameAsync(Scope scope, string? name, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask SetDisplayNamesAsync(Scope scope, ImmutableDictionary<CultureInfo, string> names, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask SetNameAsync(Scope scope, string? name, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask SetPropertiesAsync(Scope scope, ImmutableDictionary<string, JsonElement> properties, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask SetResourcesAsync(Scope scope, ImmutableArray<string> resources, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask UpdateAsync(Scope scope, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
