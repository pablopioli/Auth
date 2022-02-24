using MemoryStorage.Domain;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using System.Collections.Immutable;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using SR = OpenIddict.Abstractions.OpenIddictResources;

namespace MemoryStorage.Stores;

public class AuthorizationStore : IOpenIddictAuthorizationStore<Authorization>
{
    private static List<Authorization>? Authorizations;
    private readonly string? _storageFileName;

    public AuthorizationStore(IOptions<OpenIddictMemoryStorageOptions> storageOptions)
    {
        if (Authorizations == null)
        {
            if (string.IsNullOrEmpty(storageOptions.Value.AuthorizationFileStorage))
            {
                Authorizations = new();
            }
            else
            {
                _storageFileName = Path.GetFullPath(storageOptions.Value.AuthorizationFileStorage);

                if (File.Exists(_storageFileName))
                {
                    var content = File.ReadAllText(_storageFileName);
                    Authorizations = JsonSerializer.Deserialize<List<Authorization>>(content);
                }
                else
                {
                    Authorizations = new();
                }
            }
        }
    }

    public ValueTask<long> CountAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask<long> CountAsync<TResult>(Func<IQueryable<Authorization>, IQueryable<TResult>> query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask CreateAsync(Authorization authorization, CancellationToken cancellationToken)
    {
        Check.NotNull(authorization, nameof(authorization));

        if (Authorizations != null)
        {
            Authorizations.Add(authorization);
            SaveAuthorizations();
        }

        return ValueTask.CompletedTask;
    }

    public ValueTask DeleteAsync(Authorization authorization, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<Authorization> FindAsync(string subject, string client, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<Authorization> FindAsync(string subject, string client, string status, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<Authorization> FindAsync(string subject, string client, string status, string type, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<Authorization> FindAsync(string subject, string client, string status, string type, ImmutableArray<string> scopes, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<Authorization> FindByApplicationIdAsync(string identifier, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask<Authorization?> FindByIdAsync(string identifier, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(identifier))
        {
            throw new ArgumentException(SR.GetResourceString(SR.ID0195), nameof(identifier));
        }

        return ValueTask.FromResult((Authorizations ?? new List<Authorization>()).Find(x => x.Id == identifier));
    }

    public IAsyncEnumerable<Authorization> FindBySubjectAsync(string subject, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask<string?> GetApplicationIdAsync(Authorization authorization, CancellationToken cancellationToken)
    {
        Check.NotNull(authorization, nameof(authorization));

        if (string.IsNullOrEmpty(authorization.ApplicationId))
        {
            return ValueTask.FromResult<string?>(null);
        }

        return ValueTask.FromResult<string?>(authorization.ApplicationId);
    }

    public ValueTask<TResult> GetAsync<TState, TResult>(Func<IQueryable<Authorization>, TState, IQueryable<TResult>> query, TState state, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask<DateTimeOffset?> GetCreationDateAsync(Authorization authorization, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask<string?> GetIdAsync(Authorization authorization, CancellationToken cancellationToken)
    {
        Check.NotNull(authorization, nameof(authorization));

        return new ValueTask<string?>(authorization.Id);
    }

    public async ValueTask<ImmutableDictionary<string, JsonElement>> GetPropertiesAsync(Authorization authorization, CancellationToken cancellationToken)
    {
        Check.NotNull(authorization, nameof(authorization));

        if (authorization.Properties is null)
        {
            return await new ValueTask<ImmutableDictionary<string, JsonElement>>(ImmutableDictionary.Create<string, JsonElement>());
        }

        using var document = JsonDocument.Parse(authorization.Properties);
        var builder = ImmutableDictionary.CreateBuilder<string, JsonElement>();

        foreach (var property in document.RootElement.EnumerateObject())
        {
            builder[property.Name] = property.Value.Clone();
        }

        return await new ValueTask<ImmutableDictionary<string, JsonElement>>(builder.ToImmutable());
    }

    public ValueTask<ImmutableArray<string>> GetScopesAsync(Authorization authorization, CancellationToken cancellationToken)
    {
        Check.NotNull(authorization, nameof(authorization));

        if (authorization.Scopes is null || authorization.Scopes.Count == 0)
        {
            return new ValueTask<ImmutableArray<string>>(ImmutableArray.Create<string>());
        }

        return new ValueTask<ImmutableArray<string>>(authorization.Scopes.ToImmutableArray());
    }

    public ValueTask<string?> GetStatusAsync(Authorization authorization, CancellationToken cancellationToken)
    {
        Check.NotNull(authorization, nameof(authorization));
        return new ValueTask<string?>(authorization.Status);
    }

    public ValueTask<string?> GetSubjectAsync(Authorization authorization, CancellationToken cancellationToken)
    {
        Check.NotNull(authorization, nameof(authorization));
        return new ValueTask<string?>(authorization.Subject);
    }

    public ValueTask<string?> GetTypeAsync(Authorization authorization, CancellationToken cancellationToken)
    {
        Check.NotNull(authorization, nameof(authorization));
        return new ValueTask<string?>(authorization.Type);
    }

    public ValueTask<Authorization> InstantiateAsync(CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(new Authorization(Guid.NewGuid().ToString()));
    }

    public IAsyncEnumerable<Authorization> ListAsync(int? count, int? offset, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<TResult> ListAsync<TState, TResult>(Func<IQueryable<Authorization>, TState, IQueryable<TResult>> query, TState state, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask PruneAsync(DateTimeOffset threshold, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask SetApplicationIdAsync(Authorization authorization, string? identifier, CancellationToken cancellationToken)
    {
        Check.NotNull(authorization, nameof(authorization));

        if (!string.IsNullOrEmpty(identifier))
        {
            authorization.ApplicationId = identifier;
        }
        else
        {
            authorization.ApplicationId = string.Empty;
        }

        return default;
    }

    public ValueTask SetCreationDateAsync(Authorization authorization, DateTimeOffset? date, CancellationToken cancellationToken)
    {
        Check.NotNull(authorization, nameof(authorization));

        if (date.HasValue)
        {
            authorization.CreationDate = date.Value.UtcDateTime;
        }

        return default;
    }

    public ValueTask SetPropertiesAsync(Authorization authorization, ImmutableDictionary<string, JsonElement> properties, CancellationToken cancellationToken)
    {
        Check.NotNull(authorization, nameof(authorization));

        if (properties?.IsEmpty != false)
        {
            authorization.Properties = null;

            return default;
        }

        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Indented = false
        });

        writer.WriteStartObject();

        foreach (var property in properties)
        {
            writer.WritePropertyName(property.Key);
            property.Value.WriteTo(writer);
        }

        writer.WriteEndObject();
        writer.Flush();

        authorization.Properties = Encoding.UTF8.GetString(stream.ToArray());

        return default;
    }

    public ValueTask SetScopesAsync(Authorization authorization, ImmutableArray<string> scopes, CancellationToken cancellationToken)
    {
        Check.NotNull(authorization, nameof(authorization));

        if (scopes.IsDefaultOrEmpty)
        {
            authorization.Scopes = ImmutableList.Create<string>();

            return default;
        }

        authorization.Scopes = scopes.ToImmutableList();

        return default;
    }

    public ValueTask SetStatusAsync(Authorization authorization, string? status, CancellationToken cancellationToken)
    {
        Check.NotNull(authorization, nameof(authorization));

        authorization.Status = status ?? "";

        return default;
    }

    public ValueTask SetSubjectAsync(Authorization authorization, string? subject, CancellationToken cancellationToken)
    {
        Check.NotNull(authorization, nameof(authorization));

        authorization.Subject = subject ?? "";

        return default;
    }

    public ValueTask SetTypeAsync(Authorization authorization, string? type, CancellationToken cancellationToken)
    {
        Check.NotNull(authorization, nameof(authorization));

        authorization.Type = type ?? "";

        return default;
    }

    public ValueTask UpdateAsync(Authorization authorization, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private void SaveAuthorizations()
    {
        if (!string.IsNullOrEmpty(_storageFileName))
        {
            File.WriteAllText(_storageFileName, JsonSerializer.Serialize(Authorizations));
        }
    }
}
