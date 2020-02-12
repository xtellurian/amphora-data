using System.Collections.Generic;

namespace Amphora.Api.Contracts
{
    public interface IAmphoraeTextAnalysisService
    {
        string GetCacheKey();
        List<List<object>> ToWordSizeList(IDictionary<string, int> freqs);
        Dictionary<string, int> WordFrequencies(int maxWords = 200);
    }
}