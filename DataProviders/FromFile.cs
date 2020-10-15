using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Media_Rename.DataProviders
{
    class FromFile : IDataProvider
    {
        static Regex SeasonEpisodeRegex = new Regex(" S([0-9]{2,})E([0-9]{2,})(?: |$)", RegexOptions.IgnoreCase);
        static Regex EpisodeRegex = new Regex(" ([0-9]{2,})(?: |$)", RegexOptions.IgnoreCase);

        public XDG.XDG XDG { get; set; }
        public IConfigurationSection Config { get; set; }

        public bool IsAvailable => true;

        public Task<ImmutableDictionary<string, string>> Process(ImmutableDictionary<string, string> input)
        {
            var output = new Dictionary<string, string>();
            var attr = new FileInfo(input["filepath"]);
            output["file.name"] = Path.GetFileNameWithoutExtension(input["filepath"]);
            output["file.size"] = attr.Length.ToString();
            return Task.FromResult(output.ToImmutableDictionary());
        }
    }
}
