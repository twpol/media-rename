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
            Application = application;
        }

        public string GetConfig(params string[] path) => FormatPath(Path.Combine(new[] { GetConfigHome(), Application }.Concat(path).ToArray()));
        public string GetCache(params string[] path) => FormatPath(Path.Combine(new[] { GetCacheHome(), Application }.Concat(path).ToArray()));
        public string GetData(params string[] path) => FormatPath(Path.Combine(new[] { GetDataHome(), Application }.Concat(path).ToArray()));

        static readonly Regex WordStart = new Regex("(?<!\\.)\\b[a-zA-Z]");

        static string FormatPath(string path)
        {
            // Canonicalise the path to match the OS conventions
            // Windows: Title Case
            // Linux: kebab-case
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return WordStart.Replace(path.Replace('-', ' '), match => match.Value.ToUpperInvariant());
            }
            else
            {
                return WordStart.Replace(path, match => match.Value.ToLowerInvariant()).Replace(' ', '-');
            }
        }

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
