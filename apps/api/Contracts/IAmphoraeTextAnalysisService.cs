using System.Collections.Generic;

namespace Amphora.Api.Contracts
{
    public interface IAmphoraeTextAnalysisService
    {
        List<List<object>> ToWordSizeList(IDictionary<string, int> freqs);
        Dictionary<string, int> WordFrequencies();
    }
}