using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace PgpSharp
{
    /// <summary>
    /// Contains configuration values for using GnuPG.
    /// </summary>
    public static class GpgConfig
    {
        /// <summary>
        /// The GPG executable path key for appSettings.
        /// </summary>
        public const string GpgExePathKey = "pgpsharp:GpgExePath";

        private static string __gpgExePath = TryFindGpgPath();

        private static string TryFindGpgPath()
        {
            // try config first, otherwise search typical gpg install folder
            var exe = ConfigurationManager.AppSettings[GpgExePathKey];

            if (!File.Exists(exe))
            {
                var folder = GetDefaultGpgInstallFolder();

                exe = Path.Combine(folder, "gpg.exe");
                if (!File.Exists(exe))
                {
                    exe = Path.Combine(folder, "gpg2.exe");
                    if (!File.Exists(exe))
                    {
                        return null;
                    }
                }
            }
            return exe;
        }

        private static string GetDefaultGpgInstallFolder()
        {
            var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Gnu\\GnuPG");
            if (IntPtr.Size == 8 && !Directory.Exists(folder))
            {
                // try x86 version on 64bit OS
                folder = Path.Combine(Environment.GetEnvironmentVariable("ProgramFiles(x86)"), "Gnu\\GnuPG");
            }
            return folder;
        }

        /// <summary>
        /// Gets or sets the GPG executable path.
        /// </summary>
        /// <value>
        /// The GPG executable path.
        /// </value>
        public static string GpgExePath
        {
            get { return __gpgExePath; }
            set
            {
                if (File.Exists(value))
                {
                    __gpgExePath = value;
                }
            }
        }

    }
}
