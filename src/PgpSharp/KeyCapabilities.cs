using System;

namespace PgpSharp;

/// <summary>
/// Indicates what the key can do.
/// </summary>
[Flags]
public enum KeyCapabilities
{
    /// <summary>
    /// Not applicable to current key.
    /// </summary>
    None = 0,
    /// <summary>
    /// Key can encrypt.
    /// </summary>
    Encrypt = 0x1,
    /// <summary>
    /// Key can sign.
    /// </summary>
    Sign = 0x2,
    /// <summary>
    /// Key can certify
    /// </summary>
    Certify = 0x4,
    /// <summary>
    /// Key can perform authentication
    /// </summary>
    Authentication = 0x8,
    /// <summary>
    /// Unknown capability
    /// </summary>
    Unknown = 0x10,
    /// <summary>
    /// Key is disabled.
    /// </summary>
    Disabled = 0x20
}