using System;
using System.IO;

namespace PgpSharp.GnuPG;

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

    private static string? TryFindGpgPath()
    {
        // search typical gpg install folder
        var folders = new[]
        {
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "GnuPG\\bin"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Gnu\\GnuPG"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "GnuPG\\bin"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Gnu\\GnuPG")
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
}