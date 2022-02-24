using MemoryStorage.Domain;
using System.Text.Json;

namespace MemoryStorage.DataSource;

public class ApplicationDataSource
{
    private readonly List<Application> _applications = new();
    public IReadOnlyList<Application> Applications => _applications;

    public void Add(Application application)
    {
        _applications.Add(application);
    }

    public void Remove(Application application)
    {
        _applications.Remove(application);
    }

    public static ApplicationDataSource FromFile(string file)
    {
        var app = new ApplicationDataSource();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var storedApps = JsonSerializer.Deserialize<List<Contracts.Application>>(File.ReadAllText(file), options);

        if (storedApps != null)
        {
            foreach (var storedApp in storedApps)
            {
                app.Add(storedApp.ToApplication());
            }
        }

        return app;
    }
}
