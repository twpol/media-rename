using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Media_Rename.DataProviders
{
    internal interface IDataProvider
    {
        XDG.XDG XDG { get; set; }
        IConfigurationSection Config { get; set; }
        bool IsAvailable { get; }
        Task<ImmutableDictionary<string, string>> Process(ImmutableDictionary<string, string> input);
    }
}
