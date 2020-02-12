using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;

namespace Amphora.Api.Pages
{
    public class IndexModel : PageModel
    {
        private const string TextAnalysisKey = nameof(TextAnalysisKey);
        private readonly IBlobCache blobCache;
        private readonly IAmphoraeTextAnalysisService textAnalysisService;

        public IndexModel(IBlobCache blobCache, IAmphoraeTextAnalysisService textAnalysisService)
        {
            this.blobCache = blobCache;
            this.textAnalysisService = textAnalysisService;
        }

        public List<List<object>> Frequencies { get; set; } = new List<List<object>>();
        public int MaxWordCount { get; set; } = 0;
        public int MinWordCount { get; set; } = 0;

        public async Task<IActionResult> OnGetAsync()
        {
            var cacheEntry = await blobCache.TryGetValue<Dictionary<string, int>>(textAnalysisService.GetCacheKey());
            if (cacheEntry != null)
            {
                Frequencies = textAnalysisService.ToWordSizeList(cacheEntry);
                if (cacheEntry.Values.Count > 0)
                {
                    MaxWordCount = cacheEntry.Values.Max();
                    MinWordCount = cacheEntry.Values.Min();
                }
            }

            return Page();
        }
    }
}