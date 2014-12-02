using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PgpSharp
{
    /// <summary>
    /// Implements <see cref="IPgpTool"/> as a gnupg cli wrapper.
    /// </summary>
    public class GpgTool : IPgpTool
    {
        #region IPgpTool Members

        /// <summary>
        /// Processes the stream input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>
        /// Output stream.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">input</exception>
        /// <exception cref="PgpException"></exception>
        public Stream Process(StreamProcessInput input)
        {
            if (input == null) { throw new ArgumentNullException("input"); }
            input.Verify();
            throw new NotImplementedException();
        }

        /// <summary>
        /// Processes the file input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <exception cref="System.ArgumentNullException">input</exception>
        /// <exception cref="PgpException"></exception>
        public void Process(FileProcessInput input)
        {
            if (input == null) { throw new ArgumentNullException("input"); }
            input.Verify();
            throw new NotImplementedException();
        }

        #endregion
    }
}
