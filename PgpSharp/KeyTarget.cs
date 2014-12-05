using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PgpSharp
{
    /// <summary>
    /// Indicates the key listing target.
    /// </summary>
    public enum KeyTarget
    {
        /// <summary>
        /// The public keys.
        /// </summary>
        Public,
        /// <summary>
        /// The private keys.
        /// </summary>
        Secret
    }
}
