﻿using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json.Serialization;

namespace OpenIddict.MemoryStorage.Domain;

/// <summary>
/// Represents an OpenIddict scope.
/// </summary>
[DebuggerDisplay("Id = {Id.ToString(),nq} ; Name = {Name,nq}")]
public class Scope
{
    /// <summary>
    /// Initialize a new <see cref="Scope"/>.
    /// </summary>
    public Scope(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentNullException(nameof(id));
        }

        Id = id;
    }

    /// <summary>
    /// Initialize a new <see cref="Scope"/> from another.
    /// </summary>
    public Scope(Scope other)
    {
        Id = other.Id;
        Description = other.Description;
        Descriptions = other.Descriptions;
        DisplayName = other.DisplayName;
        DisplayNames = other.DisplayNames;
        Name = other.Name;
        Properties = other.Properties;
        Resources = other.Resources;
    }

    /// <summary>
    /// Gets or sets the client identifier used to find the client in the database.
    /// </summary>
    [JsonPropertyName("id")]
    public virtual string Id { get; internal set; }

    /// <summary>
    /// Gets or sets the public description associated with the current scope.
    /// </summary>
    [JsonPropertyName("description")]
    public virtual string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the localized public descriptions associated with the current scope.
    /// </summary>
    [JsonPropertyName("descriptions")]
    public virtual IReadOnlyDictionary<CultureInfo, string> Descriptions { get; set; }
        = ImmutableDictionary.Create<CultureInfo, string>();

    /// <summary>
    /// Gets or sets the display name associated with the current scope.
    /// </summary>
    [JsonPropertyName("display_name")]
    public virtual string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the localized display names associated with the current scope.
    /// </summary>
    [JsonPropertyName("display_names")]
    public virtual IReadOnlyDictionary<CultureInfo, string> DisplayNames { get; set; }
        = ImmutableDictionary.Create<CultureInfo, string>();

    /// <summary>
    /// Gets or sets the unique name associated with the current scope.
    /// </summary>
    [JsonPropertyName("name")]
    public virtual string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the additional properties associated with the current scope.
    /// </summary>
    [JsonPropertyName("properties")]
    public virtual string Properties { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the resources associated with the current scope.
    /// </summary>
    [JsonPropertyName("resources")]
    public virtual IReadOnlyList<string> Resources { get; set; } = ImmutableList.Create<string>();
}
