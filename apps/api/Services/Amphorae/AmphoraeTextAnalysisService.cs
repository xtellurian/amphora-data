using System.Collections.Generic;
using System.Linq;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Amphorae;

namespace Amphora.Api.Services.Amphorae
{
    public class AmphoraeTextAnalysisService : IAmphoraeTextAnalysisService
    {
        private readonly IEntityStore<AmphoraModel> amphoraStore;
        private string[] blacklist = { "a", "an", "on", "of", "or", "as", "i", "in", "is", "to", "the", "and", "for", "with", "not", "by" };

        public AmphoraeTextAnalysisService(IEntityStore<AmphoraModel> amphoraStore)
        {
            this.amphoraStore = amphoraStore;
        }

        public Dictionary<string, int> WordFrequencies()
        {
            var allText = new List<string>();
            // use name, description, labels, and attributes.
            foreach (var a in amphoraStore.Query(_ => true))
            {
                allText.AddRange(RemoveBlacklist(WordsFromText(a.Name)));
                allText.AddRange(RemoveBlacklist(WordsFromText(a.Description)));
                allText.AddRange(RemoveBlacklist(a.FileAttributes.Keys.ToList()));
                allText.AddRange(RemoveBlacklist(Flatten(a.V2Signals.Select(_ => _.Attributes.Attributes.Keys))));
            }

            var result = CountWords(allText);
            return result;
        }

        public List<List<object>> ToWordSizeList(IDictionary<string, int> freqs)
        {
            var result = new List<List<object>>();
            foreach (var kvp in freqs)
            {
                result.Add(new List<object> { kvp.Key, kvp.Value });
            }

            return result;
        }

        private List<string> WordsFromText(string text)
        {
            return text.ToLower().Split(' ').ToList();
        }

        private List<string> RemoveBlacklist(List<string> words)
        {
            return words.Where(x => x.Length > 2).Where(x => !blacklist.Contains(x)).ToList();
        }

        private List<string> Flatten(IEnumerable<IEnumerable<string>> strings)
        {
            var result = new List<string>();
            foreach (var s in strings)
            {
                result.AddRange(s);
            }

            return result;
        }

        private Dictionary<string, int> CountWords(List<string> words)
        {
            var result = new Dictionary<string, int>();
            var keyWords = words.GroupBy(x => x).OrderByDescending(x => x.Count());
            foreach (var word in keyWords)
            {
                result.Add(word.Key, word.Count());
            }

            return result;
        }
    }
}
