using System.Diagnostics;
using System.Text.Json.Serialization;

namespace MemoryStorage.Domain;

/// <summary>
/// Represents an OpenIddict token.
/// </summary>
[DebuggerDisplay("Id = {Id.ToString(),nq} ; Subject = {Subject,nq} ; Type = {Type,nq} ; Status = {Status,nq}")]
public class Token
{
    /// <summary>
    /// Initialize a new <see cref="Token"/>.
    /// </summary>
    [JsonConstructor]
    public Token(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentNullException(nameof(id));
        }

        Id = id;
    }

    /// <summary>
    /// Initialize a new <see cref="Token"/> from another.
    /// </summary>
    public Token(Token other)
    {
        Id = other.Id;
        ApplicationId = other.ApplicationId;
        AuthorizationId = other.AuthorizationId;
        CreationDate = other.CreationDate;
        ExpirationDate = other.ExpirationDate;
        Payload = other.Payload;
        Properties = other.Properties;
        RedemptionDate = other.RedemptionDate;
        ReferenceId = other.ReferenceId;
        Status = other.Status;
        Subject = other.Subject;
        Type = other.Type;
    }

    /// <summary>
    /// Gets or sets the client identifier used to find the client in the database.
    /// </summary>
    [JsonPropertyName("id")]
    public virtual string Id { get; internal set; }

    /// <summary>
    /// Gets or sets the identifier of the application associated with the current token.
    /// </summary>
    [JsonPropertyName("application_id")]
    public virtual string ApplicationId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the identifier of the authorization associated with the current token.
    /// </summary>
    [JsonPropertyName("authorization_id")]
    public virtual string AuthorizationId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the UTC creation date of the current token.
    /// </summary>
    [JsonPropertyName("creation_date")]
    public virtual DateTime? CreationDate { get; set; }

    /// <summary>
    /// Gets or sets the UTC expiration date of the current token.
    /// </summary>
    [JsonPropertyName("expiration_date")]
    public virtual DateTime? ExpirationDate { get; set; }

    /// <summary>
    /// Gets or sets the payload of the current token, if applicable.
    /// Note: this property is only used for reference tokens
    /// and may be encrypted for security reasons.
    /// </summary>
    [JsonPropertyName("payload")]
    public virtual string Payload { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the additional properties associated with the current token.
    /// </summary>
    [JsonPropertyName("properties")]
    public virtual string? Properties { get; set; }

    /// <summary>
    /// Gets or sets the UTC redemption date of the current token.
    /// </summary>
    [JsonPropertyName("redemption_date")]
    public virtual DateTime? RedemptionDate { get; set; }

    /// <summary>
    /// Gets or sets the reference identifier associated
    /// with the current token, if applicable.
    /// Note: this property is only used for reference tokens
    /// and may be hashed or encrypted for security reasons.
    /// </summary>
    [JsonPropertyName("reference_id")]
    public virtual string ReferenceId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the status of the current token.
    /// </summary>
    [JsonPropertyName("status")]
    public virtual string Status { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the subject associated with the current token.
    /// </summary>
    [JsonPropertyName("subject")]
    public virtual string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of the current token.
    /// </summary>
    [JsonPropertyName("type")]
    public virtual string Type { get; set; } = string.Empty;
}
