using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace PgpSharp.GnuPG
{
    [TestClass]
    public class TempFileStreamTests
    {
        [TestMethod]
        public void Deletes_File_On_Dispose()
        {
            string tempFile = Path.GetTempFileName();
            Assert.IsTrue(File.Exists(tempFile), "Temp file not created.");

            using (var ts = new TempFileStream(tempFile)) { }
            Assert.IsTrue(!File.Exists(tempFile), "Temp file not deleted.");
        }
    }
}
