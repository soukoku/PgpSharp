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
        /// Pushes the <see cref="SecureString"/> char-by-char to the specified input.
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
                input.Flush();
            }
        }


        public static void CopyStream(Stream input, Stream output)
        {
            CopyStream(input, output, 4096);
        }
        public static void CopyStream(Stream input, Stream output, int bufferSize)
        {
            byte[] buff = new byte[bufferSize];
            int read = 0;
            while ((read = input.Read(buff, 0, buff.Length)) > 0)
            {
                output.Write(buff, 0, read);
            }
        }
    }
}
