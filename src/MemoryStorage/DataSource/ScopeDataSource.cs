using MemoryStorage.Domain;

namespace MemoryStorage.DataSource;

public class ScopeDataSource
{
    private readonly List<Scope> _scopes = new();
    public IReadOnlyList<Scope> Scopes => _scopes;

    public void Add(Scope scope)
    {
        _scopes.Add(scope);
    }
}
