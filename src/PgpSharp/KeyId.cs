using System.Collections.Generic;
using System.Linq;

namespace PgpSharp;

/// <summary>
/// Simple id for a key.
/// </summary>
public class KeyId
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KeyId" /> class.
    /// </summary>
    /// <param name="keyId">The key id.</param>
    /// <param name="fingerprint">The finger print.</param>
    /// <param name="allowedCapability">The allowed capability.</param>
    /// <param name="usableCapability">The usable capability.</param>
    /// <param name="userIds">The user ids.</param>
    public KeyId(string keyId, string fingerprint, 
        KeyCapabilities allowedCapability, 
        KeyCapabilities usableCapability, 
        IEnumerable<string> userIds)
    {
        Id = keyId;
        Fingerprint = fingerprint;
        AllowedCapability = allowedCapability;
        UsableCapability = usableCapability;
        UserIds = userIds;
    }

    /// <summary>
    /// Gets the key id.
    /// </summary>
    public string Id { get; private set; }

    /// <summary>
    /// Gets the associated user ids.
    /// </summary>
    public IEnumerable<string> UserIds { get; private set; }

    /// <summary>
    /// Gets the fingerprint.
    /// </summary>
    public string Fingerprint { get; private set; }

    /// <summary>
    /// Gets the allowed capability for the key.
    /// </summary>
    public KeyCapabilities AllowedCapability { get; private set; }

    /// <summary>
    /// Gets the usable capability for the key.
    /// </summary>
    public KeyCapabilities UsableCapability { get; private set; }

    /// <summary>
    /// Performs an implicit conversion from <see cref="KeyId"/> to <see cref="System.String"/>.
    /// </summary>
    /// <param name="keyId">The identifier.</param>
    /// <returns>
    /// The result of the conversion.
    /// </returns>
    public static implicit operator string(KeyId keyId) => keyId?.ToString();

    /// <summary>
    /// Returns a <see cref="System.String" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String" /> that represents this instance.
    /// </returns>
    public override string ToString() => $"{UserIds.FirstOrDefault()} ({Id})";
}