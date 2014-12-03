using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;
using System.Security;

namespace PgpSharp.GnuPG
{
    [TestClass]
    public class GpgToolTests
    {
        // This assumes there's a key-pair created in gpg keyring aleady.
        // Params used to create the test key are:
        // Name: Tester
        // Email: tester@test.com
        // Comment: for testing purposes only
        // Passphrase: test123

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            __samplesFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Samples");
            if (!Directory.Exists(__samplesFolder))
            {
                Assert.Fail("Samples folder not found.");
            }
            __passphrase = Util.MakeSecureString("test123");
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            __passphrase.Dispose();
        }

        static string __samplesFolder;
        const string TESTER_NAME = "tester";
        static SecureString __passphrase;


        [TestMethod]
        public void Can_Encrypt_And_Decrypt_Text_File()
        {
            string origFile = Path.Combine(__samplesFolder, "OriginalText.txt");
            string encryptedFile = Path.Combine(__samplesFolder, "OriginalText_Encrypted.pgp");
            string decryptedFile = Path.Combine(__samplesFolder, "OriginalText_Decrypted.txt");
            Util.CleanFiles(encryptedFile, decryptedFile);

            var encryptArg = new FileDataInput
            {
                Armorize = true,
                InputFile = origFile,
                OutputFile = encryptedFile,
                Operation = DataOperation.Encrypt,
                Recipient = TESTER_NAME,
            };

            GpgTool tool = new GpgTool();
            tool.ProcessData(encryptArg);

            Assert.IsTrue(File.Exists(encryptedFile), "Encrypted file not found.");


            var decryptArg = new FileDataInput
            {
                InputFile = encryptedFile,
                OutputFile = decryptedFile,
                Operation = DataOperation.Decrypt,
                Recipient = TESTER_NAME,
                Passphrase = __passphrase
            };
            tool.ProcessData(decryptArg);

            Assert.IsTrue(File.Exists(decryptedFile), "Decrypted file not found.");

            string origText = File.ReadAllText(origFile);
            string finalText = File.ReadAllText(decryptedFile);
            Assert.AreEqual(origText, finalText, "Roundtrip got diffent file.");
            Util.CleanFiles(encryptedFile, decryptedFile);
        }

        [TestMethod]
        public void Can_Encrypt_And_Decrypt_Binary_File()
        {
            string origFile = Path.Combine(__samplesFolder, "OriginalBinary.png");
            string encryptedFile = Path.Combine(__samplesFolder, "OriginalBinary_Encrypted.pgp");
            string decryptedFile = Path.Combine(__samplesFolder, "OriginalBinary_Decrypted.png");
            Util.CleanFiles(encryptedFile, decryptedFile);

            var encryptArg = new FileDataInput
            {
                Armorize = true,
                InputFile = origFile,
                OutputFile = encryptedFile,
                Operation = DataOperation.Encrypt,
                Recipient = TESTER_NAME,
            };

            GpgTool tool = new GpgTool();
            tool.ProcessData(encryptArg);

            Assert.IsTrue(File.Exists(encryptedFile), "Encrypted file not found.");


            var decryptArg = new FileDataInput
            {
                InputFile = encryptedFile,
                OutputFile = decryptedFile,
                Operation = DataOperation.Decrypt,
                Recipient = TESTER_NAME,
                Passphrase = __passphrase
            };
            tool.ProcessData(decryptArg);

            Assert.IsTrue(File.Exists(decryptedFile), "Decrypted file not found.");

            byte[] origBytes = File.ReadAllBytes(origFile);
            byte[] finalBytes = File.ReadAllBytes(decryptedFile);
            CollectionAssert.AreEqual(origBytes, finalBytes, "Roundtrip got diffent file.");
            Util.CleanFiles(encryptedFile, decryptedFile);
        }
    }
}
