using System.Collections.Generic;
using System.IO;

namespace PgpSharp;

/// <summary>
/// Interface for a PGP-compatible tool.
/// </summary>
public interface IPgpTool
{
    // /// <summary>
    // /// Processes data with stream input.
    // /// </summary>
    // /// <param name="input">The input.</param>
    // /// <returns>Output stream.</returns>
    // Stream ProcessData(StreamDataInput input);

    /// <summary>
    /// Processes data with file input.
    /// </summary>
    /// <param name="input">The input.</param>
    void ProcessData(FileDataInput input);

    /// <summary>
    /// Lists the known keys.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns></returns>
    IEnumerable<KeyId> ListKeys(KeyTarget target);
}