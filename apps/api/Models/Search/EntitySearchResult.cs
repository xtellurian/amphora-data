using System.Collections.Generic;
using Newtonsoft.Json;

namespace Amphora.Api.Models.Search
{
    // Microsoft.Azure.Search.Models.DocumentSearchResult<T>
    public class EntitySearchResult<T>
    {
        public EntitySearchResult() {}
        public EntitySearchResult(IList<SearchResult<T>> results)
        {
            this.Results = results;
        }
        public EntitySearchResult(IEnumerable<T> entities)
        {
            this.Results = new List<SearchResult<T>>();
            foreach(var e in entities)
            {
                this.Results.Add(new SearchResult<T>(e));
            }
        }
        public string ContinuationToken { get; set; }
        [JsonProperty(PropertyName = "@odata.count")]
        public long? Count { get; set; }
        [JsonProperty(PropertyName = "@search.coverage")]
        public double? Coverage { get; set; }

        //[JsonProperty(PropertyName = "@search.facets")]
        // public IDictionary<string, IList<FacetResult>> Facets { get; }
        public IList<SearchResult<T>> Results { get; set; }
    }
}
