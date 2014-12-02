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
        /// Processes the stream input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Output stream.</returns>
        Stream Process(StreamProcessInput input);

        /// <summary>
        /// Processes the file input.
        /// </summary>
        /// <param name="input">The input.</param>
        void Process(FileProcessInput input);
    }
}
