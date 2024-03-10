using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.IO;
using System.Security;

namespace PgpSharp
{
    [TestClass]
    public class IOExtensionsTests
    {
        #region WriteSecret

        [TestMethod]
        public void WriteSecret_Should_Ignore_Null_Or_Empty()
        {
            using (var ss = new SecureString())
            {
                // test with empty
                StringBuilder sb = new StringBuilder();
                using (StringWriter sw = new StringWriter(sb))
                {
                    IOExtensions.WriteSecret(sw, ss);
                }
                Assert.IsTrue(sb.Length == 0, "Written string not empty.");
            }
        }

        [TestMethod]
        public void WriteSecret_Works_With_Ascii_Text()
        {
            string text = "simple text";

            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            using (SecureString ss = Util.MakeSecureString(text))
            {
                IOExtensions.WriteSecret(sw, ss);
                Assert.AreEqual(text, sb.ToString());
            }
        }

        [TestMethod]
        public void WriteSecret_Works_With_Non_Ascii_Text()
        {
            string text = "日本語です";

            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            using (SecureString ss = Util.MakeSecureString(text))
            {
                IOExtensions.WriteSecret(sw, ss);
                Assert.AreEqual(text, sb.ToString());
            }
        }

        #endregion

        #region DecodeAsciiEscapes

        [TestMethod]
        public void DecodeAsciiEscapes_Handles_Null()
        {
            string? input = null;
            var output = IOExtensions.DecodeAsciiEscapes(input);
            Assert.AreEqual(input, output);
        }

        [TestMethod]
        public void DecodeAsciiEscapes_Handles_No_Escapes()
        {
            var input = "hello world!";
            var output = IOExtensions.DecodeAsciiEscapes(input);
            Assert.AreEqual(input, output);
        }

        [TestMethod]
        public void DecodeAsciiEscapes_Handles_One_Escapes()
        {
            var input = @"hello\x20world!";
            var output = IOExtensions.DecodeAsciiEscapes(input);
            Assert.AreEqual("hello world!", output);
        }

        [TestMethod]
        public void DecodeAsciiEscapes_Handles_Multiple_Escapes()
        {
            // also use this test for begin and end
            var input = @"\x40hello\x20world!\x3a";
            var output = IOExtensions.DecodeAsciiEscapes(input);
            Assert.AreEqual("@hello world!:", output);
        }

        [TestMethod]
        public void DecodeAsciiEscapes_Handles_Ambiguous_Escapes()
        {
            var input = @"\x4010 cats say\x3aa lol cat.";
            var output = IOExtensions.DecodeAsciiEscapes(input);
            Assert.AreEqual("@10 cats say:a lol cat.", output);
        }

        #endregion
    }
}
