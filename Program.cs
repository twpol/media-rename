using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Media_Rename.DataProviders;
using Microsoft.Extensions.Configuration;
using CLP = CommandLineParser;

namespace Media_Rename
{
    class Program
    {
        static readonly XDG.XDG XDG = new XDG.XDG("Media Rename");

        static async Task Main(string[] args)
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

                await Process(new ConfigurationBuilder()
                    .AddJsonFile(config.Value.FullName, true)
                    .Build());
            }
            catch (CLP.Exceptions.CommandLineException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static async Task Process(IConfigurationRoot config)
        {
            var renames = config.GetSection("Rename").GetChildren();
            foreach (var rename in renames)
            {
                Console.WriteLine($"{rename.Key}: {rename["Source"]} --> {rename["Destination"]}");

                var dataProviders = new IDataProvider[]
                {
                    new FromFile() { XDG = XDG, Config = rename.GetSection("FromFile"), },
                    new FromFilename() { XDG = XDG, Config = rename.GetSection("FromFilename"), },
                };

                foreach (var file in GetMediaFiles(rename["Source"]))
                {
                    var data = ImmutableDictionary<string, string>.Empty.Add("filepath", file);
                    Console.WriteLine($"    {file}");
                    foreach (var provider in dataProviders)
                        data = data.Concat(await provider.Process(data)).ToImmutableDictionary();
                    foreach (var kvp in data)
                        Console.WriteLine($"        {kvp.Key} = {kvp.Value}");
                    Console.WriteLine();
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
