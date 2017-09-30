using System.IO;
using System.Security;

namespace PgpSharp
{
    /// <summary>
    /// Base input object for data processing.
    /// </summary>
    public abstract class DataInput
    {
        /// <summary>
        /// Gets or sets the operation.
        /// </summary>
        /// <value>
        /// The operation.
        /// </value>
        public DataOperation Operation { get; set; }

        /// <summary>
        /// Gets or sets the originator. Required to sign.
        /// </summary>
        /// <value>
        /// The originator.
        /// </value>
        public string Originator { get; set; }

        /// <summary>
        /// Gets or sets the recipient. Required to encrypt.
        /// </summary>
        /// <value>
        /// The recipient.
        /// </value>
        public string Recipient { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the output should be ASCII armored.
        /// </summary>
        /// <value>
        ///   <c>true</c> to armorize; otherwise, <c>false</c>.
        /// </value>
        public bool Armor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the public key should always be trusted.
        /// </summary>
        /// <value>
        ///   <c>true</c> to always trust the public key; otherwise, <c>false</c>.
        /// </value>
        public bool AlwaysTrustPublicKey { get; set; }
       

        /// <summary>
        /// Gets or sets the passphrase for the operation. Required to sign and decrypt.
        /// </summary>
        /// <value>
        /// The passphrase.
        /// </value>
        public SecureString Passphrase { get; set; }

        /// <summary>
        /// Gets a value indicating whether a passphrase is required for the specified <see cref="Operation"/>.
        /// </summary>
        /// <value>
        ///   <c>true</c> if passphrase is needed; otherwise, <c>false</c>.
        /// </value>
        public bool NeedsPassphrase
        {
            get
            {
                switch (Operation)
                {
                    case DataOperation.Decrypt:
                    case DataOperation.SignAndEncrypt:
                    case DataOperation.Sign:
                    case DataOperation.ClearSign:
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Verifies this input for requirements.
        /// </summary>
        /// <exception cref="PgpSharp.PgpException"></exception>
        public virtual void Verify()
        {
            switch (Operation)
            {
                case DataOperation.Decrypt:
                    RequirePasspharse();
                    break;
                case DataOperation.Encrypt:
                    RequireRecipient();
                    break;
                case DataOperation.Sign:
                case DataOperation.ClearSign:
                    RequireOriginator();
                    RequirePasspharse();
                    break;
                case DataOperation.SignAndEncrypt:
                    RequireOriginator();
                    RequireRecipient();
                    RequirePasspharse();
                    break;
                default:
                    throw new PgpException($"Unknown operation {Operation}.");
            }
        }

        private void RequirePasspharse()
        {
            if (Passphrase == null || Passphrase.Length == 0)
            {
                throw new PgpException($"Passphrase is required for {Operation} operation.");
            }
        }

        private void RequireOriginator()
        {
            if (string.IsNullOrEmpty(Originator))
            {
                throw new PgpException($"Originator is required for {Operation} operation.");
            }
        }

        private void RequireRecipient()
        {
            if (string.IsNullOrEmpty(Recipient))
            {
                throw new PgpException($"Recipient is required for {Operation} operation.");
            }
        }
    }

    /// <summary>
    /// Input object for processing a stream.
    /// </summary>
    public class StreamDataInput : DataInput
    {
        /// <summary>
        /// Gets or sets the input data stream.
        /// </summary>
        /// <value>
        /// The input data.
        /// </value>
        public Stream InputData { get; set; }

        /// <summary>
        /// Verifies this input for requirements.
        /// </summary>
        /// <exception cref="PgpSharp.PgpException">Input data is required.</exception>
        public override void Verify()
        {
            if (InputData == null)
            {
                throw new PgpException("Input data is required.");
            }

            base.Verify();
        }
    }

    /// <summary>
    /// Input object for processing a file.
    /// </summary>
    public class FileDataInput : DataInput
    {
        /// <summary>
        /// Gets or sets the input file path.
        /// </summary>
        /// <value>
        /// The input file.
        /// </value>
        public string InputFile { get; set; }

        /// <summary>
        /// Gets or sets the output file path.
        /// </summary>
        /// <value>
        /// The output file.
        /// </value>
        public string OutputFile { get; set; }

        /// <summary>
        /// Verifies this input for requirements.
        /// </summary>
        /// <exception cref="PgpSharp.PgpException">
        /// Input file is required.
        /// or
        /// Output file is required.
        /// </exception>
        public override void Verify()
        {
            if (string.IsNullOrEmpty(InputFile))
            {
                throw new PgpException("Input file is required.");
            }
            if (string.IsNullOrEmpty(OutputFile))
            {
                throw new PgpException("Output file is required.");
            }

            base.Verify();
        }
    }
}
