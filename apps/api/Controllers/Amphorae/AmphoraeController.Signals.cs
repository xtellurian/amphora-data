using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Api.Extensions;
using Amphora.Api.Models;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Api.Options;
using Amphora.Common.Models.Amphorae;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NSwag.Annotations;

namespace Amphora.Api.Controllers.Amphorae
{
    [ApiMajorVersion(0)]
    [ApiController]
    [SkipStatusCodePages]
    [Produces("application/json")]
    [Route("api/amphorae/{id}")]
    [OpenApiTag("Amphorae")]
    public class AmphoraeSignalsController : Controller
    {
        private readonly IAmphoraeService amphoraeService;
        private readonly ISignalService signalService;
        private readonly IMapper mapper;
        private readonly IOptionsMonitor<SignalOptions> options;

        public AmphoraeSignalsController(IAmphoraeService amphoraeService,
                                         ISignalService signalService,
                                         IMapper mapper,
                                         IOptionsMonitor<SignalOptions> options)
        {
            this.amphoraeService = amphoraeService;
            this.signalService = signalService;
            this.mapper = mapper;
            this.options = options;
        }

        /// <summary>
        /// Get's the signals associated with an Amphora.
        /// </summary>
        /// <param name="id">Amphora Id.</param>
        /// <returns>A collection of signals.</returns>
        [Produces(typeof(List<Signal>))]
        [HttpGet("signals")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetSignals(string id)
        {
            var result = await amphoraeService.ReadAsync(User, id, true);
            if (result.Succeeded)
            {
                var amphora = result.Entity;
                var res = new List<Signal>();
                foreach (var s in amphora.V2Signals)
                {
                    res.Add(new Signal
                    {
                        Id = s.Id,
                        ValueType = s.ValueType,
                        Property = s.Property,
                        Attributes = s.Attributes?.Attributes
                    });
                }

                return Ok(res);
            }
            else if (result.WasForbidden)
            {
                return StatusCode(403, result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }

        /// <summary>
        /// Associates a signal with an Amphora. Signal is created if not existing.
        /// </summary>
        /// <param name="id">Amphora Id.</param>
        /// <param name="signal">Signal Details.</param>
        /// <returns>Signal metadata.</returns>
        [Produces(typeof(Signal))]
        [HttpPost("signals")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CreateSignal(string id, [FromBody] Signal signal)
        {
            try
            {
                var result = await amphoraeService.ReadAsync(User, id, true);
                if (result.Succeeded)
                {
                    var amphora = result.Entity;
                    var newSignal = new SignalV2(signal.Property, signal.ValueType);
                    string message;
                    if (amphora.TryAddSignal(newSignal, out message))
                    {
                        var updateRes = await amphoraeService.UpdateAsync(User, amphora);
                        if (updateRes.Succeeded)
                        {
                            // happy path
                            signal.Id = newSignal.Id;
                            return Ok(signal);
                        }
                        else if (result.WasForbidden)
                        {
                            return StatusCode(403, result.Message);
                        }
                        else
                        {
                            return BadRequest(result.Message);
                        }
                    }
                    else
                    {
                        return BadRequest(message);
                    }
                }
                else if (result.WasForbidden)
                {
                    return StatusCode(403, result.Message);
                }
                else
                {
                    return BadRequest(result.Message);
                }
            }
            catch (System.ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Associates a signal with an Amphora. Signal is created if not existing.
        /// </summary>
        /// <param name="id">Amphora Id.</param>
        /// <param name="signalId">Signal Details.</param>
        /// <param name="signal">Signal properties to update.</param>
        /// <returns>Signal metadata.</returns>
        [Produces(typeof(Signal))]
        [HttpPut("signals/{signalId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateSignal(string id, string signalId, [FromBody] UpdateSignal signal)
        {
            var result = await amphoraeService.ReadAsync(User, id);
            if (result.Succeeded)
            {
                var amphora = result.Entity;
                var existingSignal = amphora.V2Signals.FirstOrDefault(s => s.Id == signalId);
                if (existingSignal == null)
                {
                    return BadRequest("Signal not found");
                }

                existingSignal.Attributes = new AttributeStore(signal.Meta);
                var updateRes = await amphoraeService.UpdateAsync(User, amphora);
                if (updateRes.Succeeded)
                {
                    return Ok(new Signal(existingSignal.Id, existingSignal.Property, existingSignal.ValueType, existingSignal.Attributes.Attributes));
                }
                else { return BadRequest(updateRes.Message); }
            }
            else if (result.WasForbidden)
            {
                return StatusCode(403, result.Message);
            }
            else
            {
                return NotFound(result.Message);
            }
        }

        /// <summary>
        /// Uploads values to an Amphora signal(s).
        /// </summary>
        /// <param name="id">Amphora Id.</param>
        /// <param name="data">Signal Values.</param>
        /// <returns>The signal values.</returns>
        [HttpPost("signals/values")]
        [HttpPost("signalValues")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Produces(typeof(Dictionary<string, object>))]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UploadSignal(string id, [FromBody] Dictionary<string, object> data)
        {
            var result = await amphoraeService.ReadAsync(User, id, true);
            if (result.Succeeded)
            {
                var res = await signalService.WriteSignalAsync(User, result.Entity, data);
                if (res.Succeeded)
                {
                    return Ok(res.Entity);
                }
                else { return BadRequest(res.Message); }
            }
            else if (result.WasForbidden)
            {
                return StatusCode(403, result.Message);
            }
            else
            {
                return NotFound(result.Message);
            }
        }

        /// <summary>
        /// Uploads values in batch to an Amphora signal(s).
        /// </summary>
        /// <param name="id">Amphora Id.</param>
        /// <param name="data">Signal Values.</param>
        /// <returns>A collection of signal values.</returns>
        [HttpPost("signals/batchvalues")] // TODO: make obsolete
        [HttpPost("batchSignalValues")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Produces(typeof(Dictionary<string, object>))]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UploadSignalBatch(string id, [FromBody] List<Dictionary<string, object>> data)
        {
            var result = await amphoraeService.ReadAsync(User, id, true);
            if (result.Succeeded)
            {
                var res = await signalService.WriteSignalBatchAsync(User, result.Entity, data);
                if (res.Succeeded)
                {
                    return Ok(res.Entity);
                }
                else { return BadRequest(res.Message); }
            }
            else if (result.WasForbidden)
            {
                return StatusCode(403, result.Message);
            }
            else
            {
                return NotFound(result.Message);
            }
        }

        private IActionResult NotFoundOrForbidden(EntityOperationResult<AmphoraModel> result)
        {
            if (result.WasForbidden)
            {
                return StatusCode(403, result.Message);
            }
            else
            {
                return NotFound(result.Message);
            }
        }
    }
}
