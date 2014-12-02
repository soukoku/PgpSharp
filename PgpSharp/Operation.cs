using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PgpSharp
{
    /// <summary>
    /// Defines the supported IO operations.
    /// </summary>
    public enum Operation
    {
        Sign,
        Verify,
        Encrypt,
        Decrypt,
        SignAndEncrypt
    }
}
