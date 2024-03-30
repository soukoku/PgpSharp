using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace PgpSharp.Gpg;

/// <summary>
/// Implements <see cref="IPgpTool"/> as a gnupg cli wrapper.
/// </summary>
public class GpgTool : IPgpTool
{
    public GpgTool(GpgOptions options)
    {
        Options = options;
    }

    /// <summary>
    /// Gets the options used by this tool.
    /// </summary>
    public GpgOptions Options { get; }


    #region IPgpTool Members

    // /// <summary>
    // /// Processes data with stream input.
    // /// </summary>
    // /// <param name="input">The input.</param>
    // /// <returns>
    // /// Output stream.
    // /// </returns>
    // /// <exception cref="System.ArgumentNullException">input</exception>
    // /// <exception cref="PgpException"></exception>
    // public Stream ProcessData(StreamDataInput input)
    // {
    //     if (input == null)
    //     {
    //         throw new ArgumentNullException("input");
    //     }
    //
    //     input.CheckRequirements();
    //
    //     // only way to reliably make this work is save to file and process it instead.
    //     string tempInFile = null;
    //     string tempOutFile = null;
    //     try
    //     {
    //         tempInFile = Path.GetTempFileName();
    //         tempOutFile = Path.GetTempFileName();
    //
    //         using (var fs = File.OpenWrite(tempInFile))
    //         {
    //             input.InputData.CopyTo(fs);
    //         }
    //
    //         var newArg = new FileDataInput
    //         {
    //             Armor = input.Armor,
    //             AlwaysTrustPublicKey = input.AlwaysTrustPublicKey,
    //             InputFile = tempInFile,
    //             Operation = input.Operation,
    //             Originator = input.Originator,
    //             OutputFile = tempOutFile,
    //             Passphrase = input.Passphrase,
    //             Recipient = input.Recipient
    //         };
    //         ProcessData(newArg);
    //         return new FileStream(tempOutFile, FileMode.Open, FileAccess.Read, FileShare.None, 4096,
    //             FileOptions.DeleteOnClose);
    //     }
    //     catch
    //     {
    //         IOExtensions.DeleteFiles(tempOutFile);
    //         throw;
    //     }
    //     finally
    //     {
    //         IOExtensions.DeleteFiles(tempInFile);
    //     }
    // }

    /// <summary>
    /// Processes data with file input.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <exception cref="System.ArgumentNullException">input</exception>
    /// <exception cref="PgpException"></exception>
    public void ProcessData(FileDataInput input)
    {
        if (input == null)
        {
            throw new ArgumentNullException(nameof(input));
        }

        input.CheckRequirements();
        Options.Validate();

        using (var proc = new RedirectedProcess(Options.GpgPath!, CreateDataCommandLineArgs(input)))
        {
            if (proc.Start())
            {
                if (input.NeedsPassphrase)
                {
                    proc.Input.WriteSecret(input.Passphrase!);
                    proc.Input.Write(Environment.NewLine);
                    proc.Input.Flush();
                }

                proc.WaitForExit();

                Debug.WriteLine("gpg output: " + proc.Output);
                Debug.WriteLine("gpg exit code: " + proc.ExitCode);
                var error = proc.Error;
                if (!string.IsNullOrEmpty(error) && proc.ExitCode != 0)
                {
                    Debug.WriteLine("gpg error: " + error);
                    throw new PgpException(error);
                }
            }
        }
    }


    string CreateDataCommandLineArgs(DataInput input)
    {
        // yes to all confirmations, batch for no prompt
        StringBuilder args = new StringBuilder("--yes --batch --no-greeting ");
        if (input.Armor)
        {
            args.Append("-a ");
        }

        if (input.AlwaysTrustPublicKey)
        {
            // args.Append("--always-trust ");
            args.Append("--trust-model always ");
        }

        if (!string.IsNullOrWhiteSpace(Options.KeyringFolder))
        {
            args.AppendFormat("--homedir \"{0}\" ", Options.KeyringFolder);
        }

        // only supports 4 main operations here
        if (input.Operation.HasFlag(DataOperation.Encrypt))
        {
            args.Append("--encrypt ");

            if (input.Operation.HasFlag(DataOperation.Sign))
            {
                args.Append("--sign ");
            }
            else if (input.Operation.HasFlag(DataOperation.ClearSign))
            {
                args.Append("--clearsign ");
            }
        }
        else if (input.Operation.HasFlag(DataOperation.Sign))
        {
            args.Append("--sign ");
        }
        else if (input.Operation.HasFlag(DataOperation.ClearSign))
        {
            args.Append("--clearsign ");
        }
        else if (input.Operation.HasFlag(DataOperation.Decrypt))
        {
            // decrypt can implicitly verify
            args.Append("--decrypt ");
        }
        else if (input.Operation.HasFlag(DataOperation.Verify))
        {
            args.Append("--verify ");
        }
        else if (input.Operation.HasFlag(DataOperation.DetachSign))
        {
            args.Append("--detach-sign ");
        }

        if (input.NeedsRecipient)
        {
            args.AppendFormat("-r \"{0}\" ", input.Recipient);
        }

        if (input.NeedsOriginator)
        {
            args.AppendFormat("-u \"{0}\" ", input.Originator);
        }

        if (input.NeedsPassphrase)
        {
            // will enter passphrase via std in
            // args.Append("--passphrase-fd 0 ");
            args.Append("--pinentry-mode loopback --passphrase-fd 0 ");
        }

        var fileInput = input as FileDataInput;
        if (fileInput != null)
        {
            args.AppendFormat("-o \"{0}\" \"{1}\"", fileInput.OutputFile, fileInput.InputFile);
        }

        var finalArg = args.ToString();
        Debug.WriteLine("gpg args: " + finalArg);

        return finalArg;
    }

    /// <summary>
    /// Lists the known keys.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns></returns>
    public IEnumerable<KeyId> ListKeys(KeyTarget target)
    {
        Options.Validate();

        var args = "--fixed-list-mode --with-colons --with-fingerprint --list-public-keys";
        var keyHead = "pub";
        if (target == KeyTarget.Secret)
        {
            args = "--fixed-list-mode --with-colons --with-fingerprint --list-secret-keys";
            keyHead = "sec";
        }

        if (!string.IsNullOrWhiteSpace(Options.KeyringFolder))
        {
            args += string.Format(" --homedir \"{0}\" ", Options.KeyringFolder);
        }

        using (var proc = new RedirectedProcess(Options.GpgPath!, args))
        {
            if (proc.Start())
            {
                proc.WaitForExit();

                string[] lines = proc.Output.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < lines.Length; i++)
                {
                    var fields = lines[i].Split(':');
                    if (fields[0] == keyHead)
                    {
                        var id = fields[4];
                        var allowCap = KeyCapabilities.None;
                        var useCap = KeyCapabilities.None;
                        ParseKeyCapField(fields[11], ref allowCap, ref useCap);
                        i++;

                        // read more lines as part of this key
                        string finger = string.Empty;
                        List<string> users = new();
                        for (; i < lines.Length; i++)
                        {
                            fields = lines[i].Split(':');
                            if (fields[0] == keyHead)
                            {
                                // another key, exit loop
                                i--;
                                break;
                            }

                            switch (fields[0])
                            {
                                case "uid":
                                    users.Add(fields[9].DecodeAsciiEscapes()!);
                                    break;
                                case "fpr":
                                    finger = fields[9];
                                    break;
                            }
                        }

                        yield return new KeyId(id, finger, allowCap, useCap, users);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Parses the key capability field from list keys command.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="allowed">The allowed.</param>
    /// <param name="usable">The usable.</param>
    static void ParseKeyCapField(string field, ref KeyCapabilities allowed, ref KeyCapabilities usable)
    {
        foreach (char c in field)
        {
            switch (c)
            {
                case 'e':
                    allowed |= KeyCapabilities.Encrypt;
                    break;
                case 'E':
                    usable |= KeyCapabilities.Encrypt;
                    break;
                case 's':
                    allowed |= KeyCapabilities.Sign;
                    break;
                case 'S':
                    usable |= KeyCapabilities.Sign;
                    break;
                case 'c':
                    allowed |= KeyCapabilities.Certify;
                    break;
                case 'C':
                    usable |= KeyCapabilities.Certify;
                    break;
                case 'a':
                    allowed |= KeyCapabilities.Authentication;
                    break;
                case 'A':
                    usable |= KeyCapabilities.Authentication;
                    break;
                case '?':
                    allowed |= KeyCapabilities.Unknown;
                    break;
                case 'D':
                    usable |= KeyCapabilities.Disabled;
                    break;
            }
        }
    }

    #endregion
}