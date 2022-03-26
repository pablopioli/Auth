using System.Collections.Immutable;
using System.Globalization;
using System.Text.Json;

namespace MemoryStorage.Domain;

/// <summary>
/// Represents an OpenIddict application.
/// </summary>
public class Application
{
    public Application()
    {
        Id = Guid.NewGuid().ToString();
    }

    public Application(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentNullException(nameof(id));
        }

        Id = id;
    }

    public Application(Application other)
    {
        Id = other.Id;
        ClientId = other.ClientId;
        ClientSecret = other.ClientSecret;
        ConsentType = other.ConsentType;
        DisplayName = other.DisplayName;
        DisplayNames = other.DisplayNames;
        Permissions = other.Permissions;
        PostLogoutRedirectUris = other.PostLogoutRedirectUris;
        Properties = other.Properties;
        RedirectUris = other.RedirectUris;
        Requirements = other.Requirements;
        Type = other.Type;
    }

    /// <summary>
    /// Gets or sets the client identifier used to find the client in the database.
    /// </summary>
    public virtual string Id { get; internal set; }

    /// <summary>
    /// Gets or sets the client identifier associated with the current application.
    /// </summary>
    public virtual string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the client secret associated with the current application.
    /// Note: depending on the application manager used to create this instance,
    /// this property may be hashed or encrypted for security reasons.
    /// </summary>
    public virtual string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the consent type associated with the current application.
    /// </summary>
    public virtual string ConsentType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name associated with the current application.
    /// </summary>
    public virtual string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the localized display names associated with the current application.
    /// </summary>
    public virtual IReadOnlyDictionary<CultureInfo, string> DisplayNames { get; set; }
        = ImmutableDictionary.Create<CultureInfo, string>();

    /// <summary>
    /// Gets or sets the permissions associated with the current application.
    /// </summary>
    public virtual IReadOnlyList<string> Permissions { get; set; } = ImmutableList.Create<string>();

    /// <summary>
    /// Gets or sets the logout callback URLs associated with the current application.
    /// </summary>
    public virtual IReadOnlyList<string> PostLogoutRedirectUris { get; set; } = ImmutableList.Create<string>();

    /// <summary>
    /// Gets or sets the additional properties associated with the current application.
    /// </summary>
    public virtual JsonElement Properties { get; set; }

    /// <summary>
    /// Gets or sets the callback URLs associated with the current application.
    /// </summary>
    public virtual IReadOnlyList<string> RedirectUris { get; set; } = ImmutableList.Create<string>();

    /// <summary>
    /// Gets or sets the requirements associated with the current application.
    /// </summary>
    public virtual IReadOnlyList<string> Requirements { get; set; } = ImmutableList.Create<string>();

    /// <summary>
    /// Gets or sets the application type
    /// associated with the current application.
    /// </summary>
    public virtual string Type { get; set; } = string.Empty;

    /// <summary>
    /// Set the domains excepted from CORS, when you use a browser call
    /// </summary>
    public virtual IReadOnlyList<string> CorsDomains { get; set; } = ImmutableList.Create<string>();
}
