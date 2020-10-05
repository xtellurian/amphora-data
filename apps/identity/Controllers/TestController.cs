using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Identity.Controllers
{
    [SkipStatusCodePages]
    [ApiController]
    public class TestController : Controller
    {
        public TestController()
        { }

        [HttpPost("api/headers-echo")]
        public async Task<IActionResult> Get()
        {
            await Task.Delay(1); // just make it async for warnings
            var headers = new Dictionary<string, string>();
            foreach (var h in Request.Headers)
            {
                headers.Add(h.Key, h.Value);
            }

            return Ok(headers);
        }
    }
}