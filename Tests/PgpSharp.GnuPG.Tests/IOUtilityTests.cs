using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.IO;
using System.Security;

namespace PgpSharp.GnuPG
{
    [TestClass]
    public class IOUtilityTests
    {
        [TestMethod]
        public void PushSecret_Should_Ignore_Null_Or_Empty()
        {
            // throw no exceptions with null args
            IOUtility.PushSecret(null, null);
            using (var ss = new SecureString())
            {
                IOUtility.PushSecret(null, ss);

                // test with empty
                StringBuilder sb = new StringBuilder();
                using (StringWriter sw = new StringWriter(sb))
                {
                    IOUtility.PushSecret(sw, ss);
                }
                Assert.IsTrue(sb.Length == 0, "Written string not empty.");
            }
        }

        [TestMethod]
        public void PushSecret_Works_With_Ascii_Text()
        {
            string text = "simple text";

            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            using (SecureString ss = Util.MakeSecureString(text))
            {
                IOUtility.PushSecret(sw, ss);
                // add new line due to the push adding it
                Assert.AreEqual(text + Environment.NewLine, sb.ToString());
            }
        }

        [TestMethod]
        public void PushSecret_Works_With_Non_Ascii_Text()
        {
            string text = "日本語です";

            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            using (SecureString ss = Util.MakeSecureString(text))
            {
                IOUtility.PushSecret(sw, ss);
                Assert.AreEqual(text + Environment.NewLine, sb.ToString());
            }
        }
    }
}
