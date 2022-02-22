using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenIddict.MemoryStorage.Domain;

/// <summary>
/// Represents an OpenIddict application.
/// </summary>
[DebuggerDisplay("Id = {Id.ToString(),nq} ; ClientId = {ClientId,nq} ; Type = {Type,nq}")]
public class Application
{
    /// <summary>
    /// Initialize a new <see cref="CouchDbOpenIddictToken"/>.
    /// </summary>
    public Application()
    {
        Id = Guid.NewGuid().ToString();
    }

    /// <summary>
    /// Initialize a new <see cref="CouchDbOpenIddictToken"/>.
    /// </summary>
    public Application(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentNullException(nameof(id));
        }

        Id = id;
    }

    /// <summary>
    /// Initialize a new <see cref="CouchDbOpenIddictApplication"/> from another.
    /// </summary>
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
    [JsonPropertyName("id")]
    public virtual string Id { get; internal set; }

    /// <summary>
    /// Gets or sets the client identifier associated with the current application.
    /// </summary>
    [JsonPropertyName("client_id")]
    public virtual string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the client secret associated with the current application.
    /// Note: depending on the application manager used to create this instance,
    /// this property may be hashed or encrypted for security reasons.
    /// </summary>
    [JsonPropertyName("client_secret")]
    public virtual string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the consent type associated with the current application.
    /// </summary>
    [JsonPropertyName("consent_type")]
    public virtual string ConsentType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name associated with the current application.
    /// </summary>
    [JsonPropertyName("display_name")]
    public virtual string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the localized display names associated with the current application.
    /// </summary>
    [JsonPropertyName("display_names")]
    public virtual IReadOnlyDictionary<CultureInfo, string> DisplayNames { get; set; }
        = ImmutableDictionary.Create<CultureInfo, string>();

    /// <summary>
    /// Gets or sets the permissions associated with the current application.
    /// </summary>
    [JsonPropertyName("permissions")]
    public virtual IReadOnlyList<string> Permissions { get; set; } = ImmutableList.Create<string>();

    /// <summary>
    /// Gets or sets the logout callback URLs associated with the current application.
    /// </summary>
    [JsonPropertyName("post_logout_redirect_uris")]
    public virtual IReadOnlyList<string> PostLogoutRedirectUris { get; set; } = ImmutableList.Create<string>();

    /// <summary>
    /// Gets or sets the additional properties associated with the current application.
    /// </summary>
    [JsonPropertyName("properties")]
    public virtual JsonElement Properties { get; set; }

    /// <summary>
    /// Gets or sets the callback URLs associated with the current application.
    /// </summary>
    [JsonPropertyName("redirect_uris")]
    public virtual IReadOnlyList<string> RedirectUris { get; set; } = ImmutableList.Create<string>();

    /// <summary>
    /// Gets or sets the requirements associated with the current application.
    /// </summary>
    [JsonPropertyName("requirements")]
    public virtual IReadOnlyList<string> Requirements { get; set; } = ImmutableList.Create<string>();

    /// <summary>
    /// Gets or sets the application type
    /// associated with the current application.
    /// </summary>
    [JsonPropertyName("type")]
    public virtual string Type { get; set; } = string.Empty;
}
