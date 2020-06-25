using System.Collections.Generic;

namespace Amphora.Api.Models.Dtos.Geo
{
    public class FuzzySearchResponse
    {
        public Summary Summary { get; set; }
        public List<Result> Results { get; set; } = new List<Result>();
    }
}