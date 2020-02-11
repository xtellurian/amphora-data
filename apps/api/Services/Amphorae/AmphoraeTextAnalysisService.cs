using System;
using System.Collections.Generic;
using System.Linq;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Amphorae;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Services.Amphorae
{
    public class AmphoraeTextAnalysisService : IAmphoraeTextAnalysisService
    {
        private readonly IEntityStore<AmphoraModel> amphoraStore;
        private readonly ILogger<AmphoraeTextAnalysisService> logger;
        private string[] blacklist = { "a", "an", "on", "of", "or", "as", "i", "in", "is", "to", "the", "and", "for", "with", "not", "by", "from", "has", "been" };

        public AmphoraeTextAnalysisService(IEntityStore<AmphoraModel> amphoraStore, ILogger<AmphoraeTextAnalysisService> logger)
        {
            this.amphoraStore = amphoraStore;
            this.logger = logger;
        }

        public Dictionary<string, int> WordFrequencies(int maxWords = 200)
        {
            var allText = new List<string>();
            // use name, description, labels, and attributes.
            foreach (var a in amphoraStore.Query(_ => true))
            {
                try
                {
                    allText.AddRange(RemoveBlacklist(WordsFromText(a.Name)));
                    allText.AddRange(RemoveBlacklist(WordsFromText(a.Description)));
                    if (a.FileAttributes?.Keys != null)
                    {
                        allText.AddRange(RemoveBlacklist(a.FileAttributes?.Keys.ToList()));
                    }

                    if (a.V2Signals != null)
                    {
                        foreach (var s in a.V2Signals)
                        {
                            allText.AddRange(RemoveBlacklist(s?.Attributes?.Attributes?.Keys.ToList() ?? new List<string>()));
                            allText.AddRange(RemoveBlacklist(s?.Attributes?.Attributes?.Values.ToList() ?? new List<string>()));
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    // empty
                    logger.LogWarning(ex.Message);
                }
            }

            var result = CountWords(allText, maxWords);
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

        private Dictionary<string, int> CountWords(List<string> words, int take)
        {
            var result = new Dictionary<string, int>();
            var keyWords = words.GroupBy(x => x).OrderByDescending(x => x.Count());
            var count = 0;
            foreach (var word in keyWords)
            {
                result.Add(word.Key, word.Count());
                if (count++ >= take)
                {
                    break;
                }
            }

            return result;
        }
    }
}
