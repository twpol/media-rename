using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Media_Rename.Utils;
using Microsoft.Extensions.Configuration;

namespace Media_Rename.DataProviders
{
    class FromFilename : IDataProvider
    {
        static Regex SeasonEpisodeRegex = new Regex(" S([0-9]{2,})E([0-9]{2,})(?: |$)", RegexOptions.IgnoreCase);
        static Regex EpisodeRegex = new Regex(" ([0-9]{2,})(?: |$)", RegexOptions.IgnoreCase);

        public XDG.XDG XDG { get; set; }
        public IConfigurationSection Config { get; set; }

        public bool IsAvailable => true;

        public Task<ImmutableDictionary<string, string>> Process(ImmutableDictionary<string, string> input)
        {
            var output = new Dictionary<string, string>();
            var fileName = Filename.CleanUp(input["file.name"]);
            var seasonEpisode = SeasonEpisodeRegex.Match(fileName);
            if (seasonEpisode.Success)
            {
                output["show.name"] = Filename.CleanUp(fileName.Substring(0, seasonEpisode.Index));
                output["season.number"] = seasonEpisode.Groups[1].Value;
                output["episode.number"] = seasonEpisode.Groups[2].Value;
            }
            else
            {
                var episode = EpisodeRegex.Match(fileName);
                if (episode.Success)
                {
                    output["show.name"] = Filename.CleanUp(fileName.Substring(0, episode.Index));
                    output["episode.number"] = episode.Groups[1].Value;
                }
            }
            return Task.FromResult(output.ToImmutableDictionary());
        }
    }
}
