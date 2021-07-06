using System.Collections.Generic;
using OpenIddict.MemoryStorage.Domain;

namespace OpenIddict.MemoryStorage.DataSource
{
    public class ScopeDataSource
    {
        private readonly List<Scope> _scopes = new List<Scope>();
        public IReadOnlyList<Scope> Scopes => _scopes;

        public void Add(Scope scope)
        {
            _scopes.Add(scope);
        }
    }
}
