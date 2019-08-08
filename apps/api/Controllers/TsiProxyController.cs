using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Amphora.Api.Controllers
{
    public class TsiProxyController : Controller
    {
        private readonly ITsiService tsiService;
        private readonly ILogger<TsiProxyController> logger;

        public TsiProxyController(ITsiService tsiService, ILogger<TsiProxyController> logger)
        {
            this.tsiService = tsiService;
            this.logger = logger;
        }
        [HttpPost("tsi/timeseries/query")]
        public async Task<IActionResult> TimeSeriesQuery()
        {
            var body = await ReadBodyAsync();
            var json = JObject.Parse(body);
            var token = await tsiService.GetAccessTokenAsync();
            var content = new StringContent(JsonConvert.SerializeObject(json), Encoding.UTF8, "application/json");
            var response = await tsiService.ProxyQueryAsync($"timeseries/query{Request.QueryString}", content);

            if (response.IsSuccessStatusCode)
            {
                return Ok(await response.Content.ReadAsStringAsync());
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                logger.LogWarning(responseContent);
                return BadRequest(responseContent);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<string> ReadBodyAsync()
        {
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}
