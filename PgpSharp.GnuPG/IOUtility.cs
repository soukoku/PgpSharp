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
        /// Pushes the passphrase char-by-char to the input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="passphrase">The passphrase.</param>
        public static void PushPassphrase(StreamWriter input, SecureString passphrase)
        {
            if (passphrase != null && passphrase.Length > 0)
            {
                IntPtr ptr = Marshal.SecureStringToBSTR(passphrase);
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
