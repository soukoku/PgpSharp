using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PgpSharp
{
    /// <summary>
    /// Main interface for pgp tool.
    /// </summary>
    public interface IPgpTool
    {
        /// <summary>
        /// Processes data with stream input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Output stream.</returns>
        Stream ProcessData(StreamDataInput input);

        /// <summary>
        /// Processes data with file input.
        /// </summary>
        /// <param name="input">The input.</param>
        void ProcessData(FileDataInput input);
    }
}
