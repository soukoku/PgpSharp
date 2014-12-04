using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace PgpSharp.GnuPG
{
    static class IOUtility
    {
        /// <summary>
        /// Writes the <see cref="SecureString"/> char-by-char to the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="secret">The secret.</param>
        public static void WriteSecret(this TextWriter input, SecureString secret)
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
            }
        }

        public static void DeleteFiles(params string[] files)
        {
            if (files != null)
            {
                foreach (var f in files)
                {
                    if (File.Exists(f)) { File.Delete(f); }
                }
            }
        }
    }
}
