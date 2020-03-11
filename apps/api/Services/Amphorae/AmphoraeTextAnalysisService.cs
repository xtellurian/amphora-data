using System;
using System.Collections.Generic;
using System.Linq;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Services.Amphorae
{
    public class AmphoraeTextAnalysisService : IAmphoraeTextAnalysisService
    {
        private readonly IEntityStore<AmphoraModel> amphoraStore;
        private readonly ILogger<AmphoraeTextAnalysisService> logger;
        private string[] blacklist =
        {
            "the", "and", "for", "with", "not", "from", "has", "been", "more", "user", "both",
            "csiro", "resolution", "longitude", "longtitude", "latitude", "transformed", "includes",
            "original", "weatherzone", "ph", "available", "effort", "format", "source", "second", "modelling"
        };

        public AmphoraeTextAnalysisService(IEntityStore<AmphoraModel> amphoraStore, ILogger<AmphoraeTextAnalysisService> logger)
        {
            this.amphoraStore = amphoraStore;
            this.logger = logger;
        }

        public string GetCacheKey() => "WordFrequencies";

        public Dictionary<string, int> WordFrequencies(int maxWords = 200)
        {
            var allText = new List<string>();
            // use name, description, labels, and attributes.
            try
            {
                foreach (var a in amphoraStore.Query(_ => true))
                {
                    allText.AddRange(RemoveBlacklist(Clean(WordsFromText(a.Name))));
                    allText.AddRange(RemoveBlacklist(Clean(WordsFromText(a.Description))));
                    if (a.Labels != null && a.Labels.Count > 0)
                    {
                        allText.AddRange(RemoveBlacklist(Clean(a.Labels.Select(_ => _.Name))));
                    }

                    if (a.FileAttributes?.Keys != null)
                    {
                        allText.AddRange(RemoveBlacklist(Clean(a.FileAttributes?.Keys.ToList())));
                    }

                    if (a.V2Signals != null)
                    {
                        foreach (var s in a.V2Signals)
                        {
                            allText.AddRange(RemoveBlacklist(Clean(s?.Attributes?.Attributes?.Keys.ToList() ?? new List<string>())));
                            allText.AddRange(RemoveBlacklist(Clean(s?.Attributes?.Attributes?.Values.ToList() ?? new List<string>())));
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                // empty
                logger.LogWarning(ex.Message);
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
            return text.Split(' ').ToList();
        }

        private List<string> Clean(IEnumerable<string> words)
        {
            return words
                .Where(_ => !_.All(x => !char.IsLetter(x))) // not all chars are not letters
                .Select(_ => _?.Trim(',').Trim('.').Trim(':').Trim()) // trim punctuation and whitespace
                .Select(_ => _?.ToLower()) // make sure lowercase
                .Where(_ => !string.IsNullOrEmpty(_)) // remove empties
                .Where(x => x.Length > 2)
                .ToList();
        }

        private List<string> RemoveBlacklist(IEnumerable<string> words)
        {
            return words.Where(x => !blacklist.Contains(x)).ToList();
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
