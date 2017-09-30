using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;

namespace PgpSharp
{
    /// <summary>
    /// Contains small methods for setting up tests.
    /// </summary>
    static class Util
    {
        public static SecureString MakeSecureString(string text)
        {
            SecureString ss = new SecureString();
            foreach (char c in text)
            {
                ss.AppendChar(c);
            }
            return ss;
        }
    }
}
