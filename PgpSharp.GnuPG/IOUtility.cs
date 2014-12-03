using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace PgpSharp
{
    static class IOUtility
    {
        /// <summary>
        /// Pushes the <see cref="SecureString"/> char-by-char to the specified input
        /// and add a new-line at end to simulate enter key.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="secret">The secret.</param>
        public static void PushSecret(TextWriter input, SecureString secret)
        {
            if (input != null && secret != null && secret.Length > 0)
            {
                IntPtr ptr = Marshal.SecureStringToBSTR(secret);
                try
                {
                    int offset = 0;
                    short unichar = Marshal.ReadInt16(ptr, offset);
                    while (unichar != 0)
                    {
                        input.Write((char)unichar);
                        offset += 2;
                        unichar = Marshal.ReadInt16(ptr, offset);
                    }
                }
                finally
                {
                    Marshal.ZeroFreeBSTR(ptr);
                }
                input.Write(Environment.NewLine);
                input.Flush();
            }
        }
    }
}
