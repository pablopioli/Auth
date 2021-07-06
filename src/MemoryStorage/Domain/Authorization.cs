using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OpenIddict.MemoryStorage.Domain
{
    /// <summary>
    /// Represents an OpenIddict authorization.
    /// </summary>
    [DebuggerDisplay("Id = {Id.ToString(),nq} ; Subject = {Subject,nq} ; Type = {Type,nq} ; Status = {Status,nq}")]
    public class Authorization
    {
        internal Authorization()
        { }

        /// <summary>
        /// Initialize a new <see cref="Scope"/>.
        /// </summary>
        public Authorization(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            Id = id;
        }

        /// <summary>
        /// Initialize a new <see cref="CouchDbOpenIddictAuthorization"/> from another.
        /// </summary>
        public Authorization(Authorization other)
        {
            Id = other.Id;
            ApplicationId = other.ApplicationId;
            CreationDate = other.CreationDate;
            Properties = new JObject(other.Properties);
            Scopes = other.Scopes;
            Status = other.Status;
            Subject = other.Subject;
            Type = other.Type;
        }

        /// <summary>
        /// Gets or sets the client identifier used to find the client in the database.
        /// </summary>
        [JsonProperty("id")]
        public virtual string Id { get; internal set; }

        /// <summary>
        /// Gets or sets the identifier of the application associated with the current authorization.
        /// </summary>
        [JsonProperty("application_id")]
        public virtual string ApplicationId { get; set; } = null!;

        /// <summary>
        /// Gets or sets the UTC creation date of the current authorization.
        /// </summary>
        public virtual DateTime CreationDate { get; set; }

        /// <summary>
        /// Gets or sets the additional properties associated with the current authorization.
        /// </summary>
        [JsonProperty("properties")]
        public virtual JObject Properties { get; set; }

        /// <summary>
        /// Gets or sets the scopes associated with the current authorization.
        /// </summary>
        [JsonProperty("scopes")]
        public virtual IReadOnlyList<string> Scopes { get; set; } = ImmutableList.Create<string>();

        /// <summary>
        /// Gets or sets the status of the current authorization.
        /// </summary>
        [JsonProperty("status")]
        public virtual string Status { get; set; }

        /// <summary>
        /// Gets or sets the subject associated with the current authorization.
        /// </summary>
        [JsonProperty("subject")]
        public virtual string Subject { get; set; }

        /// <summary>
        /// Gets or sets the type of the current authorization.
        /// </summary>
        [JsonProperty("type")]
        public virtual string Type { get; set; }
    }
}
