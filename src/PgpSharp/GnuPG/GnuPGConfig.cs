using System;
using System.Configuration;
using System.IO;
using System.Linq;

namespace PgpSharp.GnuPG
{
    /// <summary>
    /// Contains configuration values for using GnuPG.
    /// </summary>
    public static class GnuPGConfig
    {
        /// <summary>
        /// The GPG executable path key for appSettings.
        /// </summary>
        public const string GnuPGExePathKey = "pgpsharp:GnuPGExePath";

        static readonly string __defaultGpgExePath = TryFindGpgPath();
        static string __gpgExePath;

        /// <summary>
        /// Gets or sets the GPG executable path.
        /// </summary>
        /// <value>
        /// The GPG executable path.
        /// </value>
        public static string GnuPGExePath
        {
            get { return string.IsNullOrEmpty(__gpgExePath) ? __defaultGpgExePath : __gpgExePath; }
            set
            {
                if (File.Exists(value))
                {
                    __gpgExePath = value;
                }
            }
        }

        private static string TryFindGpgPath()
        {
            // try config first, otherwise search typical gpg install folder
            var exe = ConfigurationManager.AppSettings[GnuPGExePathKey];

            if (!File.Exists(exe))
            {
                var folders = new[]
                {
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "GnuPG\\bin"),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Gnu\\GnuPG"),
                    Path.Combine(Environment.GetEnvironmentVariable("ProgramFiles(x86)"), "GnuPG\\bin"),
                    Path.Combine(Environment.GetEnvironmentVariable("ProgramFiles(x86)"), "Gnu\\GnuPG")
                };

                exe = folders.Select(fdr => Path.Combine(fdr, "gpg.exe")).FirstOrDefault(f => File.Exists(f));
                if (exe == null)
                {
                    exe = folders.Select(fdr => Path.Combine(fdr, "gpg2.exe")).FirstOrDefault(f => File.Exists(f));
                }
            }
            return exe;
        }
    }
}
