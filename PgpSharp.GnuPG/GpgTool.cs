using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace PgpSharp
{
    /// <summary>
    /// Implements <see cref="IPgpTool"/> as a gnupg cli wrapper.
    /// </summary>
    public class GpgTool : IPgpTool
    {
        #region IPgpTool Members

        /// <summary>
        /// Processes the stream input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>
        /// Output stream.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">input</exception>
        /// <exception cref="PgpException"></exception>
        public Stream Process(StreamProcessInput input)
        {

            throw new NotImplementedException();

            if (input == null) { throw new ArgumentNullException("input"); }
            input.Verify();
            using (var proc = CreateProcess(input))
            {
                if (proc.Start())
                {
                    IOUtility.PushPassphrase(proc.StandardInput, input.Passphrase);
                    proc.WaitForExit();
                }
            }
        }

        /// <summary>
        /// Processes the file input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <exception cref="System.ArgumentNullException">input</exception>
        /// <exception cref="PgpException"></exception>
        public void Process(FileProcessInput input)
        {
            if (input == null) { throw new ArgumentNullException("input"); }
            input.Verify();
            using (var proc = CreateProcess(input))
            {
                if (proc.Start())
                {
                    IOUtility.PushPassphrase(proc.StandardInput, input.Passphrase);
                    proc.WaitForExit();
                }
            }
        }


        static Process CreateProcess(ProcessInput input)
        {
            StringBuilder args = new StringBuilder("--yes ");
            if (input.Armorize)
            {
                args.Append("-a ");
            }
            // todo: test args
            switch (input.Operation)
            {
                case Operation.Decrypt:
                    args.AppendFormat("-d -u \"{0}\" ", input.Recipient);
                    break;
                case Operation.Encrypt:
                    args.AppendFormat("-e -r \"{0}\" ", input.Recipient);
                    break;
                case Operation.Sign:
                    args.AppendFormat("-s -u \"{0}\" ", input.Originator);
                    break;
                case Operation.SignAndEncrypt:
                    args.AppendFormat("-s -e -r \"{0}\" -u \"{1}\" ", input.Recipient, input.Originator);
                    break;
                case Operation.Verify:
                    args.Append("--verify ");
                    break;
            }

            var fileInput = input as FileProcessInput;
            if (fileInput != null)
            {
                args.AppendFormat("-o \"{0}\" \"{1}\"", fileInput.OutputFile, fileInput.InputFile);
            }


            Process p = new Process();
            p.StartInfo.FileName = GpgConfig.GpgExePath;
            p.StartInfo.Arguments = args.ToString();
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardInput = true;

            return p;
        }

        #endregion
    }
}
