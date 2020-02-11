using System.Collections.Generic;
using System.Linq;
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
        private readonly IAmphoraeTextAnalysisService textAnalysisService;
        private readonly IMemoryCache memoryCache;
        private readonly IDateTimeProvider dateTimeProvider;

        public IndexModel(IAmphoraeTextAnalysisService textAnalysisService, IMemoryCache memoryCache, IDateTimeProvider dateTimeProvider)
        {
            this.textAnalysisService = textAnalysisService;
            this.memoryCache = memoryCache;
            this.dateTimeProvider = dateTimeProvider;
        }

        public List<List<object>> Frequencies { get; set; } = new List<List<object>>();
        public int MaxWordCount { get; set; }
        public int MinWordCount { get; set; }

        public IActionResult OnGet()
        {
            Dictionary<string, int> cacheEntry;
            if (!memoryCache.TryGetValue(TextAnalysisKey, out cacheEntry))
            {
                // Key not in cache, so get data.
                cacheEntry = textAnalysisService.WordFrequencies();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(dateTimeProvider.Now.AddHours(6));

                // Save data in cache.
                memoryCache.Set(TextAnalysisKey, cacheEntry, cacheEntryOptions);
            }

            Frequencies = textAnalysisService.ToWordSizeList(cacheEntry);
            MaxWordCount = cacheEntry.Values.Max();
            MinWordCount = cacheEntry.Values.Min();

            return Page();
        }
    }
}