using System;

namespace PgpSharp;

/// <summary>
/// Defines the supported data operations.</summary>
[Flags]
public enum DataOperation
{
    /// <summary>
    /// Unspecified operation. Cannot be used.
    /// </summary>
    Invalid = 0,

    /// <summary>
    /// Signs the data. Exclusive with <see cref="ClearSign"/>.
    /// Can be combined with <see cref="Encrypt"/>
    /// </summary>
    Sign = 0x1,

    /// <summary>
    /// Signs the data with clear text signature. Exclusive with <see cref="Sign"/>.
    /// Can be combined with <see cref="Encrypt"/>.
    /// </summary>
    ClearSign = 0x2,

    /// <summary>
    /// Signs the data with signature in a separate file (usually [filename].sig).
    /// Cannot be combined.
    /// </summary>
    DetachSign = 0x4,

    /// <summary>
    /// Encrypts the data. Can be combined with <see cref="Sign"/> or <see cref="ClearSign"/>.
    /// </summary>
    Encrypt = 0x8,

    /// <summary>
    /// Decrypt the data. Cannot be combined.
    /// </summary>
    Decrypt = 0x10,

    /// <summary>
    /// Verifies a signed data. Cannot be combined.
    /// </summary>
    Verify = 0x20,

    // /// <summary>
    // /// Sign and encrypt the data.
    // /// </summary>
    // SignAndEncrypt = Sign | Encrypt,
    //
    // /// <summary>
    // /// Sign and encrypt the data.
    // /// </summary>
    // ClearSignAndEncrypt = ClearSign | Encrypt,
}