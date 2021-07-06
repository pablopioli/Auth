using System.Collections.Generic;
using OpenIddict.MemoryStorage.Domain;

namespace OpenIddict.MemoryStorage.DataSource
{
    public class ApplicationDataSource
    {
        private readonly List<Application> _applications = new List<Application>();
        public IReadOnlyList<Application> Applications => _applications;

        public void Add(Application application)
        {
            _applications.Add(application);
        }
    }
}
