namespace PgpSharp;

/// <summary>
/// Defines the supported data operations.
/// </summary>
public enum DataOperation
{
    /// <summary>
    /// Signs the data.
    /// </summary>
    Sign,
    /// <summary>
    /// Signs the data with clear text signature.
    /// </summary>
    ClearSign,
    //Verify,
    /// <summary>
    /// Encrypts the data.
    /// </summary>
    Encrypt,
    /// <summary>
    /// Decrypt the data.
    /// </summary>
    Decrypt,
    /// <summary>
    /// Sign and encrypt the data.
    /// </summary>
    SignAndEncrypt
}