using System.Threading.Tasks;
using Amphora.Api.Contracts;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Amphora.Api.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SignalController : Controller
    {
        private readonly IAmphoraeService amphoraeService;
        private readonly ISignalService signalService;
        private readonly ITsiService tsiService;
        private readonly IMapper mapper;
        private readonly ILogger<SignalController> logger;

        public SignalController(
            IAmphoraeService amphoraeService,
            ISignalService signalService,
            ITsiService tsiService,
            IMapper mapper,
            ILogger<SignalController> logger)
        {
            this.amphoraeService = amphoraeService;
            this.signalService = signalService;
            this.tsiService = tsiService;
            this.mapper = mapper;
            this.logger = logger;
        }

        [HttpPost("api/amphorae/{id}/signals")]
        public async Task<IActionResult> Upload(string id, [FromBody] JObject jObj)
        {
            logger.LogInformation($"Signal Upload for {id}");
            var result = await amphoraeService.ReadAsync(User, id, true);
            if (result.Succeeded)
            {
                await signalService.WriteSignalAsync(result.Entity, jObj);
                return Ok();
            }
            else if(result.WasForbidden)
            {
                return StatusCode(403);
            }
            else
            {
                return NotFound();
            }

        }
    }
}
