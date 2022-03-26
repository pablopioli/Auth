using MemoryStorage.Domain;
using System.Text.Json;

namespace MemoryStorage.DataSource;

public class ScopeDataSource
{
    private readonly List<Scope> _scopes = new();
    public IReadOnlyList<Scope> Scopes => _scopes;

    public void Add(Scope scope)
    {
        _scopes.Add(scope);
    }

    public static ScopeDataSource FromFile(string file)
    {
        var app = new ScopeDataSource();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var scopes = JsonSerializer.Deserialize<List<Contracts.Scope>>(File.ReadAllText(file), options);

        if (scopes != null)
        {
            foreach (var scope in scopes)
            {
                app.Add(scope.ToApplication());
            }
        }

        return app;
    }
}
