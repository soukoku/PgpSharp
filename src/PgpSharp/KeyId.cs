using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace PgpSharp
{
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
        public KeyId(string keyId, string fingerprint, KeyCapabilities allowedCapability, KeyCapabilities usableCapability, IEnumerable<string> userIds)
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
        /// <value>
        /// The key id.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets the associated user ids.
        /// </summary>
        /// <value>
        /// The user ids.
        /// </value>
        public IEnumerable<string> UserIds { get; private set; }

        /// <summary>
        /// Gets the fingerprint.
        /// </summary>
        /// <value>
        /// The fingerprint.
        /// </value>
        public string Fingerprint { get; private set; }

        /// <summary>
        /// Gets the allowed capability for the key.
        /// </summary>
        /// <value>
        /// The capability.
        /// </value>
        public KeyCapabilities AllowedCapability { get; private set; }

        /// <summary>
        /// Gets the usable capability for the key.
        /// </summary>
        /// <value>
        /// The capability.
        /// </value>
        public KeyCapabilities UsableCapability { get; private set; }

        /// <summary>
        /// Performs an implicit conversion from <see cref="KeyId"/> to <see cref="System.String"/>.
        /// </summary>
        /// <param name="keyId">The identifier.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator string (KeyId keyId)
        {
            if (keyId == null) { return null; }
            return keyId.ToString();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0} ({1})", UserIds.FirstOrDefault(), Id);
        }
    }
}
