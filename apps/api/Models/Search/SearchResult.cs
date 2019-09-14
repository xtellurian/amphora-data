using System.Collections.Generic;
using Newtonsoft.Json;

namespace Amphora.Api.Models.Search
{
    // maps from Microsoft.Azure.Search.Models.SearchResult<T>
    public class SearchResult<T>
    {
        public SearchResult() {}
        public SearchResult(T entity)
        {
            Entity = entity;
        }
        public T Entity { get; set; }
        [JsonProperty(PropertyName = "@search.score")]
        public double Score { get; set; }
        [JsonProperty(PropertyName = "@search.highlights")]
        public IDictionary<string, IList<string>> Highlights { get; set; }
    }
}