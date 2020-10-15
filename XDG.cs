using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace XDG
{
    class XDG
    {
        readonly string Application;

        public XDG(string application)
        {
            // Canonicalise the application name to match the OS conventions
            // Windows: Title Case
            // Linux: kebab-case
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Application = WordStart.Replace(application.Replace('-', ' '), match => match.Value.ToUpperInvariant());
            }
            else
            {
                Application = WordStart.Replace(application, match => match.Value.ToLowerInvariant()).Replace(' ', '-');
            }
        }

        public string GetConfig() => Path.Combine(GetConfigHome(), Application);
        public string GetCache() => Path.Combine(GetCacheHome(), Application);
        public string GetData() => Path.Combine(GetDataHome(), Application);

        static readonly Regex WordStart = new Regex("\\b[a-zA-Z]");

        static string GetConfigHome()
        {
            // XDG_CONFIG_HOME
            // Where user-specific configurations should be written (analogous to /etc).
            // Should default to $HOME/.config.
            return GetHome("XDG_CONFIG_HOME", "APPDATA", ".config");
        }

        static string GetCacheHome()
        {
            // XDG_CACHE_HOME
            // Where user-specific non-essential (cached) data should be written (analogous to /var/cache).
            // Should default to $HOME/.cache.
            return GetHome("XDG_CACHE_HOME", "LOCALAPPDATA", ".cache");
        }

        static string GetDataHome()
        {
            // XDG_DATA_HOME
            // Where user-specific data files should be written (analogous to /usr/share).
            // Should default to $HOME/.local/share.
            return GetHome("XDG_DATA_HOME", "APPDATA", Path.Combine(".local", "share"));
        }

        static string GetHome(string mainVar, string backupWinVar, string backupNixPath)
        {
            return Environment.GetEnvironmentVariable(mainVar)
                ?? (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                    ? Environment.GetEnvironmentVariable(backupWinVar)
                    : Path.Combine(Environment.GetEnvironmentVariable("HOME"), backupNixPath));

        }
    }
}
