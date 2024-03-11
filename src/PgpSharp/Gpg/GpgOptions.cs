using System;
using System.IO;

namespace PgpSharp.Gpg;

/// <summary>
/// Contains configuration values for using GnuPG.
/// </summary>
public class GpgOptions
{
    // /// <summary>
    // /// The GPG executable path key for appSettings.
    // /// </summary>
    // public const string GnuPGExePathKey = "pgpsharp:GnuPGExePath";

    static readonly string? DefaultGpgExePath = TryFindGpgPath();

    string? _gpgExePath;

    /// <summary>
    /// Gets or sets the GPG executable path.
    /// </summary>
    public string? GpgPath
    {
        get => _gpgExePath ?? DefaultGpgExePath;
        set
        {
            if (File.Exists(value))
            {
                _gpgExePath = value;
            }
        }
    }

    /// <summary>
    /// Gets or sets a different keyring folder than default.
    /// </summary>
    public string? KeyringFolder { get; set; }

    private static string? TryFindGpgPath()
    {
        // search typical gpg install folder
        var folders = new[]
        {
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "GnuPG\\bin"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Gnu\\GnuPG"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "GnuPG\\bin"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Gnu\\GnuPG"),
            "/usr/bin/",
            "/opt/homebrew/bin/"
        };
        var names = new[] { "gpg2.exe", "gpg.exe", "gpg" };

        foreach (var folder in folders)
        {
            foreach (var name in names)
            {
                var path = Path.Combine(folder, name);
                if (File.Exists(path)) return path;
            }
        }

        return null;
    }

    public void Validate()
    {
        if (!File.Exists(GpgPath))
            throw new PgpException($"Invalid gpg path '{_gpgExePath}' in the configuration.");
        if (!string.IsNullOrEmpty(KeyringFolder) && !Directory.Exists(KeyringFolder))
        {
            throw new PgpException($"Invalid keyring folder '{KeyringFolder}' in the configuration.");
        }
    }
}