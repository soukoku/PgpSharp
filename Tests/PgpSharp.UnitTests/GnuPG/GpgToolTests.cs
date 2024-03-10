using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;
using System.Security;
using System.Linq;

namespace PgpSharp.GnuPG
{
    [TestClass]
    public class GpgToolTests
    {
        // This assumes there's a key-pair created in gpg keyring for testing purposes.
        // Create this key if it doesn't exist with these params:
        // Email: tester@test.com
        // Passphrase: test123
        // can be added with gpg --quick-generate-key tester@test.com

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
        const string TESTER_NAME = "tester@test.com";
        static SecureString __passphrase;


        [TestMethod]
        public void Can_Encrypt_And_Decrypt_Text_File()
        {
            string origFile = Path.Combine(__samplesFolder, "OriginalText.txt");
            string encryptedFile = Path.Combine(__samplesFolder, "OriginalText_Encrypted.pgp");
            string decryptedFile = Path.Combine(__samplesFolder, "OriginalText_Decrypted.txt");
            IOExtensions.DeleteFiles(encryptedFile, decryptedFile);

            var encryptArg = new FileDataInput
            {
                Armor = true,
                AlwaysTrustPublicKey = true,
                InputFile = origFile,
                OutputFile = encryptedFile,
                Operation = DataOperation.Encrypt,
                Recipient = TESTER_NAME,
            };

            IPgpTool tool = new GpgTool(new GpgOptions());
            tool.ProcessData(encryptArg);

            Assert.IsTrue(File.Exists(encryptedFile), "Encrypted file not found.");


            var decryptArg = new FileDataInput
            {
                InputFile = encryptedFile,
                OutputFile = decryptedFile,
                Operation = DataOperation.Decrypt,
                Passphrase = __passphrase
            };
            tool.ProcessData(decryptArg);

            Assert.IsTrue(File.Exists(decryptedFile), "Decrypted file not found.");

            string origText = File.ReadAllText(origFile);
            string finalText = File.ReadAllText(decryptedFile);
            Assert.AreEqual(origText, finalText, "Roundtrip got diffent file.");
            IOExtensions.DeleteFiles(encryptedFile, decryptedFile);
        }

        [TestMethod]
        public void Can_Encrypt_And_Decrypt_Binary_File()
        {
            string origFile = Path.Combine(__samplesFolder, "OriginalBinary.png");
            string encryptedFile = Path.Combine(__samplesFolder, "OriginalBinary_Encrypted.pgp");
            string decryptedFile = Path.Combine(__samplesFolder, "OriginalBinary_Decrypted.png");
            IOExtensions.DeleteFiles(encryptedFile, decryptedFile);

            var encryptArg = new FileDataInput
            {
                Armor = true,
                AlwaysTrustPublicKey = true,
                InputFile = origFile,
                OutputFile = encryptedFile,
                Operation = DataOperation.Encrypt,
                Recipient = TESTER_NAME,
            };

            IPgpTool tool = new GpgTool(new GpgOptions());
            tool.ProcessData(encryptArg);

            Assert.IsTrue(File.Exists(encryptedFile), "Encrypted file not found.");


            var decryptArg = new FileDataInput
            {
                InputFile = encryptedFile,
                OutputFile = decryptedFile,
                Operation = DataOperation.Decrypt,
                Passphrase = __passphrase
            };
            tool.ProcessData(decryptArg);

            Assert.IsTrue(File.Exists(decryptedFile), "Decrypted file not found.");

            byte[] origBytes = File.ReadAllBytes(origFile);
            byte[] finalBytes = File.ReadAllBytes(decryptedFile);
            CollectionAssert.AreEqual(origBytes, finalBytes, "Roundtrip got diffent file.");
            IOExtensions.DeleteFiles(encryptedFile, decryptedFile);
        }


        // [TestMethod]
        // public void Can_Encrypt_And_Decrypt_Text_Stream()
        // {
        //     string origFile = Path.Combine(__samplesFolder, "OriginalText.txt");
        //     using (var origFs = File.OpenRead(origFile))
        //     {
        //         var encryptArg = new StreamDataInput
        //         {
        //             Armor = true,
        //             AlwaysTrustPublicKey = false,
        //             InputData = origFs,
        //             Operation = DataOperation.Encrypt,
        //             Recipient = TESTER_NAME,
        //         };
        //
        //         IPgpTool tool = new GpgTool(new GpgOptions());
        //
        //         using (var encryptStream = tool.ProcessData(encryptArg))
        //         {
        //             var decryptArg = new StreamDataInput
        //             {
        //                 InputData = encryptStream,
        //                 Operation = DataOperation.Decrypt,
        //                 Passphrase = __passphrase
        //             };
        //             using (var decryptedStream = tool.ProcessData(decryptArg))
        //             using (StreamReader reader = new StreamReader(decryptedStream))
        //             {
        //                 string origText = File.ReadAllText(origFile);
        //                 string finalText = reader.ReadToEnd();
        //                 Assert.AreEqual(origText, finalText, "Roundtrip got diffent text.");
        //             }
        //         }
        //     }
        // }
        //
        // [TestMethod]
        // public void Can_Encrypt_And_Decrypt_Binary_Stream()
        // {
        //     string origFile = Path.Combine(__samplesFolder, "OriginalBinary.png");
        //     using (var origFs = File.OpenRead(origFile))
        //     {
        //         var encryptArg = new StreamDataInput
        //         {
        //             Armor = true,
        //             AlwaysTrustPublicKey = false,
        //             InputData = origFs,
        //             Operation = DataOperation.Encrypt,
        //             Recipient = TESTER_NAME,
        //         };
        //
        //         IPgpTool tool = new GpgTool(new GpgOptions());
        //
        //         using (var encryptStream = tool.ProcessData(encryptArg))
        //         {
        //             var decryptArg = new StreamDataInput
        //             {
        //                 InputData = encryptStream,
        //                 Operation = DataOperation.Decrypt,
        //                 Passphrase = __passphrase
        //             };
        //             using (var decryptedStream = tool.ProcessData(decryptArg))
        //             using (MemoryStream testStream = new MemoryStream())
        //             {
        //                 decryptedStream.CopyTo(testStream);
        //
        //                 byte[] origBytes = File.ReadAllBytes(origFile);
        //                 byte[] finalBytes = testStream.ToArray();
        //                 CollectionAssert.AreEqual(origBytes, finalBytes, "Roundtrip got diffent bytes.");
        //             }
        //         }
        //     }
        // }


        [TestMethod]
        public void List_Public_Keys_Gets_Tester_Id()
        {
            IPgpTool tool = new GpgTool(new GpgOptions());
            var keys = tool.ListKeys(KeyTarget.Public).ToList();

            var hit = keys.FirstOrDefault(k => k.UserIds.Contains(TESTER_NAME));

            Assert.IsNotNull(hit, "Didn't find tester key id.");
        }


        [TestMethod]
        public void List_Secret_Keys_Gets_Tester_Id()
        {
            IPgpTool tool = new GpgTool(new GpgOptions());
            var keys = tool.ListKeys(KeyTarget.Secret).ToList();

            var hit = keys.FirstOrDefault(k => k.UserIds.Contains(TESTER_NAME));

            Assert.IsNotNull(hit, "Didn't find tester key id.");
        }
    }
}
