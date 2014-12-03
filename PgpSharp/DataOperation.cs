using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PgpSharp
{
    /// <summary>
    /// Defines the supported data operations.
    /// </summary>
    public enum DataOperation
    {
        Sign,
        Verify,
        Encrypt,
        Decrypt,
        SignAndEncrypt
    }
}
