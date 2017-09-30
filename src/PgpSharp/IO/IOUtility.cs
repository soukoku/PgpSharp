using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;

namespace PgpSharp.IO
{
    public static class IOUtility
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

        /// <summary>
        /// Deletes the specified files.
        /// </summary>
        /// <param name="files"></param>
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

        /// <summary>
        /// Decodes the strings containing ASCII escapes like \x3a into : (colon).
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static string DecodeAsciiEscapes(this string input)
        {
            // modified from http://stackoverflow.com/questions/11584148/how-to-convert-a-string-containing-escape-characters-to-a-string

            var rx = new Regex(@"\\x([0-9A-Fa-f]{2})");
            MatchCollection matches;
            if (input != null && (matches = rx.Matches(input)).Count > 0) // only do the work if found!
            {
                var res = new StringBuilder();
                var pos = 0;
                foreach (Match m in matches)
                {
                    res.Append(input.Substring(pos, m.Index - pos));
                    pos = m.Index + m.Length;
                    res.Append((char)Convert.ToInt32(m.Groups[1].ToString(), 16));
                }
                res.Append(input.Substring(pos));
                return res.ToString();
            }
            return input;
        }
    }
}
