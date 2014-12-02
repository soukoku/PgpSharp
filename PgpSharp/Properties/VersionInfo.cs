using System.Reflection;

[assembly: AssemblyCompany("Yin-Chun Wang")]
[assembly: AssemblyCopyright("Copyright © Yin-Chun Wang 2014")]

[assembly: AssemblyVersion(PgpSharp.VersionInfo.MajorVersion)]
[assembly: AssemblyFileVersion(PgpSharp.VersionInfo.BuildVersion)]
[assembly: AssemblyInformationalVersion(PgpSharp.VersionInfo.BuildVersion)]

namespace PgpSharp
{
    static class VersionInfo
    {
        // keep this same in major releases
        public const string MajorVersion = "0.1.0.0";
        // change this for each nuget release
        public const string BuildVersion = "0.1.0.0";
    }
}