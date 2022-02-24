using MemoryStorage.Domain;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using System.Collections.Immutable;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using SR = OpenIddict.Abstractions.OpenIddictResources;

namespace MemoryStorage.Stores;

public class TokenStore : IOpenIddictTokenStore<Token>
{
    private static List<Token>? Tokens;
    private readonly string? _storageFileName;

    public TokenStore()
    { }

    public TokenStore(IOptions<OpenIddictMemoryStorageOptions> storageOptions)
    {
        if (Tokens == null)
        {
            if (string.IsNullOrEmpty(storageOptions.Value.TokenFileStorage))
            {
                Tokens = new();
            }
            else
            {
                _storageFileName = Path.GetFullPath(storageOptions.Value.TokenFileStorage);

                if (File.Exists(_storageFileName))
                {
                    var content = File.ReadAllText(_storageFileName);
                    Tokens = JsonSerializer.Deserialize<List<Token>>(content) ?? new();
                }
                else
                {
                    Tokens = new();
                }
            }
        }
    }

    public ValueTask<long> CountAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask<long> CountAsync<TResult>(Func<IQueryable<Token>, IQueryable<TResult>> query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask CreateAsync(Token token, CancellationToken cancellationToken)
    {
        Check.NotNull(token, nameof(token));

        if (Tokens != null)
        {
            Tokens.Add(token);
            SaveTokens();
        }

        return ValueTask.CompletedTask;
    }

    public ValueTask DeleteAsync(Token token, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<Token> FindAsync(string subject, string client, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<Token> FindAsync(string subject, string client, string status, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<Token> FindAsync(string subject, string client, string status, string type, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<Token> FindByApplicationIdAsync(string identifier, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<Token> FindByAuthorizationIdAsync(string identifier, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(identifier))
        {
            throw new ArgumentException(SR.GetResourceString(SR.ID0195), nameof(identifier));
        }

        return FindByAuthorizationIdAsyncInternal();

        async IAsyncEnumerable<Token> FindByAuthorizationIdAsyncInternal()
        {
            foreach (var token in (Tokens ?? new List<Token>()).Where(x => x.ApplicationId == identifier))
            {
                yield return await Task.FromResult(token);
            }
        }
    }

    public ValueTask<Token?> FindByIdAsync(string identifier, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(identifier))
        {
            throw new ArgumentException(SR.GetResourceString(SR.ID0195), nameof(identifier));
        }

        var app = (Tokens ?? new List<Token>()).Find(x => x.Id == identifier);

        return ValueTask.FromResult(app);
    }

    public ValueTask<Token?> FindByReferenceIdAsync(string identifier, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(identifier))
        {
            throw new ArgumentException(SR.GetResourceString(SR.ID0195), nameof(identifier));
        }

        return ValueTask.FromResult((Tokens ?? new List<Token>()).Find(x => x.ReferenceId == identifier));
    }

    public IAsyncEnumerable<Token> FindBySubjectAsync(string subject, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask<string?> GetApplicationIdAsync(Token token, CancellationToken cancellationToken)
    {
        Check.NotNull(token, nameof(token));

        if (string.IsNullOrEmpty(token.ApplicationId))
        {
            return new ValueTask<string?>(result: null);
        }

        return new ValueTask<string?>(token.ApplicationId);
    }

    public ValueTask<TResult> GetAsync<TState, TResult>(Func<IQueryable<Token>, TState, IQueryable<TResult>> query, TState state, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask<string?> GetAuthorizationIdAsync(Token token, CancellationToken cancellationToken)
    {
        Check.NotNull(token, nameof(token));

        if (string.IsNullOrEmpty(token.AuthorizationId))
        {
            return new ValueTask<string?>(result: null);
        }

        return new ValueTask<string?>(token.AuthorizationId);
    }

    public ValueTask<DateTimeOffset?> GetCreationDateAsync(Token token, CancellationToken cancellationToken)
    {
        Check.NotNull(token, nameof(token));

        if (token.CreationDate is null)
        {
            DateTimeOffset? nullDateTime = null;
            return ValueTask.FromResult(nullDateTime);
        }

        return new ValueTask<DateTimeOffset?>(DateTime.SpecifyKind(token.CreationDate.Value, DateTimeKind.Utc));
    }

    public ValueTask<DateTimeOffset?> GetExpirationDateAsync(Token token, CancellationToken cancellationToken)
    {
        Check.NotNull(token, nameof(token));

        if (token.ExpirationDate is null)
        {
            return new ValueTask<DateTimeOffset?>(result: null);
        }

        return new ValueTask<DateTimeOffset?>(DateTime.SpecifyKind(token.ExpirationDate.Value, DateTimeKind.Utc));
    }

    public ValueTask<string?> GetIdAsync(Token token, CancellationToken cancellationToken)
    {
        Check.NotNull(token, nameof(token));

        return new ValueTask<string?>(token.Id);
    }

    public ValueTask<string?> GetPayloadAsync(Token token, CancellationToken cancellationToken)
    {
        Check.NotNull(token, nameof(token));

        return new ValueTask<string?>(token.Payload);
    }

    public ValueTask<ImmutableDictionary<string, JsonElement>> GetPropertiesAsync(Token token, CancellationToken cancellationToken)
    {
        Check.NotNull(token, nameof(token));

        if (token.Properties is null)
        {
            return new ValueTask<ImmutableDictionary<string, JsonElement>>(ImmutableDictionary.Create<string, JsonElement>());
        }

        using var document = JsonDocument.Parse(token.Properties);
        var builder = ImmutableDictionary.CreateBuilder<string, JsonElement>();

        foreach (var property in document.RootElement.EnumerateObject())
        {
            builder[property.Name] = property.Value.Clone();
        }

        return new ValueTask<ImmutableDictionary<string, JsonElement>>(builder.ToImmutable());
    }

    public ValueTask<DateTimeOffset?> GetRedemptionDateAsync(Token token, CancellationToken cancellationToken)
    {
        Check.NotNull(token, nameof(token));

        if (token.RedemptionDate is null)
        {
            return new ValueTask<DateTimeOffset?>(result: null);
        }

        return new ValueTask<DateTimeOffset?>(DateTime.SpecifyKind(token.RedemptionDate.Value, DateTimeKind.Utc));
    }

    public ValueTask<string?> GetReferenceIdAsync(Token token, CancellationToken cancellationToken)
    {
        Check.NotNull(token, nameof(token));
        return new ValueTask<string?>(token.ReferenceId);
    }

    public ValueTask<string?> GetStatusAsync(Token token, CancellationToken cancellationToken)
    {
        Check.NotNull(token, nameof(token));
        return new ValueTask<string?>(token.Status);
    }

    public ValueTask<string?> GetSubjectAsync(Token token, CancellationToken cancellationToken)
    {
        Check.NotNull(token, nameof(token));
        return new ValueTask<string?>(token.Subject);
    }

    public ValueTask<string?> GetTypeAsync(Token token, CancellationToken cancellationToken)
    {
        Check.NotNull(token, nameof(token));
        return new ValueTask<string?>(token.Type);
    }

    public ValueTask<Token> InstantiateAsync(CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(new Token(Guid.NewGuid().ToString()));
    }

    public IAsyncEnumerable<Token> ListAsync(int? count, int? offset, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<TResult> ListAsync<TState, TResult>(Func<IQueryable<Token>, TState, IQueryable<TResult>> query, TState state, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask PruneAsync(DateTimeOffset threshold, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask SetApplicationIdAsync(Token token, string? identifier, CancellationToken cancellationToken)
    {
        Check.NotNull(token, nameof(token));

        if (!string.IsNullOrEmpty(identifier))
        {
            token.ApplicationId = identifier;
        }
        else
        {
            token.ApplicationId = string.Empty;
        }

        return default;
    }

    public ValueTask SetAuthorizationIdAsync(Token token, string? identifier, CancellationToken cancellationToken)
    {
        Check.NotNull(token, nameof(token));

        if (!string.IsNullOrEmpty(identifier))
        {
            token.AuthorizationId = identifier;
        }
        else
        {
            token.AuthorizationId = string.Empty;
        }

        return default;
    }

    public ValueTask SetCreationDateAsync(Token token, DateTimeOffset? date, CancellationToken cancellationToken)
    {
        Check.NotNull(token, nameof(token));

        token.CreationDate = date?.UtcDateTime;

        return default;
    }

    public ValueTask SetExpirationDateAsync(Token token, DateTimeOffset? date, CancellationToken cancellationToken)
    {
        Check.NotNull(token, nameof(token));

        token.ExpirationDate = date?.UtcDateTime;

        return default;
    }

    public ValueTask SetPayloadAsync(Token token, string? payload, CancellationToken cancellationToken)
    {
        Check.NotNull(token, nameof(token));

        token.Payload = payload ?? "";

        return default;
    }

    public ValueTask SetPropertiesAsync(Token token, ImmutableDictionary<string, JsonElement> properties, CancellationToken cancellationToken)
    {
        Check.NotNull(token, nameof(token));

        if (properties?.IsEmpty != false)
        {
            token.Properties = null;
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

        token.Properties = Encoding.UTF8.GetString(stream.ToArray());

        return default;
    }

    public ValueTask SetRedemptionDateAsync(Token token, DateTimeOffset? date, CancellationToken cancellationToken)
    {
        Check.NotNull(token, nameof(token));

        token.RedemptionDate = date?.UtcDateTime;

        return default;
    }

    public ValueTask SetReferenceIdAsync(Token token, string? identifier, CancellationToken cancellationToken)
    {
        Check.NotNull(token, nameof(token));

        token.ReferenceId = identifier ?? "";

        return default;
    }

    public ValueTask SetStatusAsync(Token token, string? status, CancellationToken cancellationToken)
    {
        Check.NotNull(token, nameof(token));

        token.Status = status ?? "";

        return default;
    }

    public ValueTask SetSubjectAsync(Token token, string? subject, CancellationToken cancellationToken)
    {
        Check.NotNull(token, nameof(token));

        token.Subject = subject ?? "";

        return default;
    }

    public ValueTask SetTypeAsync(Token token, string? type, CancellationToken cancellationToken)
    {
        Check.NotNull(token, nameof(token));

        token.Type = type ?? "";

        return default;
    }

    public ValueTask UpdateAsync(Token token, CancellationToken cancellationToken)
    {
        Check.NotNull(token, nameof(token));

        if (Tokens != null)
        {
            var existingToken = Tokens.Find(x => x.Id == token.Id);
            if (existingToken != null)
            {
                Tokens.Remove(existingToken);
            }

            Tokens.Add(token);

            SaveTokens();
        }

        return ValueTask.CompletedTask;
    }

    private void SaveTokens()
    {
        if (!string.IsNullOrEmpty(_storageFileName))
        {
            File.WriteAllText(_storageFileName, JsonSerializer.Serialize(Tokens));
        }
    }
}
