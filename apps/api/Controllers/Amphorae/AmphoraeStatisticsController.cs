using System;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Amphora.Api.Controllers.Amphorae
{
    [ApiController]
    public class AmphoraeStatisticsController : Controller
    {
        private readonly IAmphoraeService amphoraeService;
        private readonly IMemoryCache memoryCache;

        public AmphoraeStatisticsController(IAmphoraeService amphoraeService, IMemoryCache memoryCache)
        {
            this.amphoraeService = amphoraeService;
            this.memoryCache = memoryCache;
        }

        private const string countKey = "amphoraeCount";

         /// <summary>
        /// Gets ta count of all the Amphora
        /// </summary>
        [Produces(typeof(int))]
        [HttpGet("api/amphoraeStats/count")]
        public async Task<IActionResult> Count()
        {
            // using a cache to prevent this public endpoint being smashed;
            int cacheEntry;
            if (!memoryCache.TryGetValue(countKey, out cacheEntry))
            {
                // Key not in cache, so get data.
                cacheEntry = await amphoraeService.AmphoraStore.CountAsync();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(DateTime.Now.AddHours(1));

                // Save data in cache.
                memoryCache.Set(countKey, cacheEntry, cacheEntryOptions);
            }
            return Ok(cacheEntry);
        }
    }
}