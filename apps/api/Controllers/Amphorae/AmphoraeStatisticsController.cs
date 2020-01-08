using System;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NSwag.Annotations;

namespace Amphora.Api.Controllers.Amphorae
{
    [ApiController]
    [SkipStatusCodePages]
    public class AmphoraeStatisticsController : Controller
    {
        private readonly IAmphoraeService amphoraeService;
        private readonly IMemoryCache memoryCache;

        public AmphoraeStatisticsController(IAmphoraeService amphoraeService, IMemoryCache memoryCache)
        {
            this.amphoraeService = amphoraeService;
            this.memoryCache = memoryCache;
        }

        private const string CountKey = "amphoraeCount";

        /// <summary>
        /// Gets the count of all the Amphora.
        /// </summary>
        /// <param name="iFrame">Boolean, whether to render as an iFrame.</param>
        /// <returns>The total number of Amphora.</returns>
        [Produces(typeof(int))]
        [HttpGet("api/amphoraeStats/count")]
        [OpenApiIgnore]
        public async Task<IActionResult> Count(bool? iFrame)
        {
            // using a cache to prevent this public endpoint being smashed;
            int cacheEntry;
            if (!memoryCache.TryGetValue(CountKey, out cacheEntry))
            {
                // Key not in cache, so get data.
                cacheEntry = await amphoraeService.AmphoraStore.CountAsync();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(DateTime.Now.AddHours(1));

                // Save data in cache.
                memoryCache.Set(CountKey, cacheEntry, cacheEntryOptions);
            }

            if (iFrame.HasValue && iFrame.Value)
            {
                string myHostUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
                var img = $"{myHostUrl}/images/Amphora_Black.svg";
                var html = $"<div> <h1> <img style=\"height:32px;\" src={img} /> {cacheEntry}</h1> </div>";
                return Content(html, "text/html");
            }
            else
            {
                return Ok(cacheEntry);
            }
        }
    }
}