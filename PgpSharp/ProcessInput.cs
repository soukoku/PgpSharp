using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;

namespace PgpSharp
{
    /// <summary>
    /// Base input object for IO processing.
    /// </summary>
    public abstract class ProcessInput
    {
        /// <summary>
        /// Gets or sets the operation.
        /// </summary>
        /// <value>
        /// The operation.
        /// </value>
        public Operation Operation { get; set; }

        /// <summary>
        /// Gets or sets the originator. Required to sign.
        /// </summary>
        /// <value>
        /// The originator.
        /// </value>
        public string Originator { get; set; }

        /// <summary>
        /// Gets or sets the recipient. Required to decrypt/encrypt.
        /// </summary>
        /// <value>
        /// The recipient.
        /// </value>
        public string Recipient { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the output should be ASCII armored.
        /// </summary>
        /// <value>
        ///   <c>true</c> to armorize; otherwise, <c>false</c>.
        /// </value>
        public bool Armorize { get; set; }

        /// <summary>
        /// Gets or sets the passphrase if the operation. Required to sign and decrypt.
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
                    case PgpSharp.Operation.Decrypt:
                    case PgpSharp.Operation.SignAndEncrypt:
                    case PgpSharp.Operation.Sign:
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
                case Operation.Decrypt:
                    RequireRecipient();
                    RequirePasspharse();
                    break;
                case Operation.Encrypt:
                    RequireRecipient();
                    break;
                case Operation.Sign:
                    RequireOriginator();
                    RequirePasspharse();
                    break;
                case Operation.SignAndEncrypt:
                    RequireOriginator();
                    RequireRecipient();
                    RequirePasspharse();
                    break;
                default:
                    throw new PgpException(string.Format("Unknown operation {0}.", Operation));
            }
        }

        private void RequirePasspharse()
        {
            if (Passphrase == null || Passphrase.Length == 0)
            {
                throw new PgpException(string.Format("Passphrase is required for {0} operation.", Operation));
            }
        }

        private void RequireOriginator()
        {
            if (string.IsNullOrEmpty(Originator))
            {
                throw new PgpException(string.Format("Originator is required for {0} operation.", Operation));
            }
        }

        private void RequireRecipient()
        {
            if (string.IsNullOrEmpty(Recipient))
            {
                throw new PgpException(string.Format("Recipient is required for {0} operation.", Operation));
            }
        }
    }

    /// <summary>
    /// Input object for processing a stream.
    /// </summary>
    public class StreamProcessInput : ProcessInput
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
    public class FileProcessInput : ProcessInput
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
