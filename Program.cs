using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using CLP = CommandLineParser;

namespace Media_Rename
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new CLP.Arguments.FileArgument('c', "config")
            {
                ForcedDefaultValue = new FileInfo("config.json"),
            };

            var commandLineParser = new CLP.CommandLineParser()
            {
                Arguments = {
                    config,
                }
            };

            try
            {
                commandLineParser.ParseCommandLine(args);

                Main(new ConfigurationBuilder()
                    .AddJsonFile(config.Value.FullName, true)
                    .Build())
                    .Wait();
            }
            catch (CLP.Exceptions.CommandLineException e)
            {
                Console.WriteLine(e.Message);
            }
        }
        static async Task Main(IConfigurationRoot config)
        {
            var renames = config.GetSection("Rename").GetChildren();
            foreach (var rename in renames)
            {
                Console.WriteLine($"{rename.Key}: {rename["Source"]} --> {rename["Destination"]}");
                foreach (var file in GetMediaFiles(rename["Source"]))
                {
                    Console.WriteLine($"    {file}");
                }
            }
        }

        static readonly string[] MEDIA_FILES = new[] {
            ".3gp",
            ".ass",
            ".avi",
            ".bmp",
            ".fla",
            ".flv",
            ".gif",
            ".idx",
            ".jpeg",
            ".jpg",
            ".m4a",
            ".m4v",
            ".mka",
            ".mkv",
            ".mov",
            ".mp4",
            ".mpa",
            ".mpeg",
            ".mpg",
            ".ogg",
            ".ogm",
            ".png",
            ".srt",
            ".sub",
            ".ts",
            ".webm",
            ".webp",
            ".wma",
            ".wmv",
            ".wtv",
        };

        static IEnumerable<string> GetMediaFiles(string path)
        {
            foreach (var file in Directory.GetFiles(path))
            {
                if (MEDIA_FILES.Contains(Path.GetExtension(file).ToLowerInvariant()))
                {
                    yield return Path.Combine(path, file);
                }
            }
            foreach (var dir in Directory.GetDirectories(path))
            {
                foreach (var file in GetMediaFiles(Path.Combine(path, dir)))
                {
                    yield return file;
                }
            }
        }
    }
}
