using System;
using System.IO;
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
            }
        }
    }
}
