using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Extensions;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Api.Options;
using Amphora.Common.Models.Signals;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Amphora.Api.Controllers.Amphorae
{
    public partial class AmphoraeController : Controller
    {
        private readonly IOptionsMonitor<SignalOptions> options;

        /// <summary>
        /// Get's the signals associated with an Amphora.
        /// </summary>
        /// <param name="id">Amphora Id</param>  
        [Produces(typeof(List<SignalDto>))]
        [HttpGet("api/amphorae/{id}/signals")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetSignals(string id)
        {
            var result = await amphoraeService.ReadAsync(User, id, true);
            if (result.Succeeded)
            {
                var signals = result.Entity.Signals.Select(s => s.Signal);
                var res = mapper.Map<List<SignalDto>>(signals);
                return Ok(signals);
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
        /// <param name="id">Amphora Id</param>  
        /// <param name="dto">Signal Details</param>  
        [Produces(typeof(SignalDto))]
        [HttpPost("api/amphorae/{id}/signals")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CreateSignal(string id, [FromBody] SignalDto dto)
        {
            try
            {
                var signal = new SignalModel(dto.Property, dto.ValueType);
                var result = await amphoraeService.ReadAsync(User, id, true);
                if (result.Succeeded)
                {
                    result.Entity.AddSignal(signal, options.CurrentValue?.MaxSignals ?? 7);
                    var updateRes = await amphoraeService.UpdateAsync(User, result.Entity);
                    if (updateRes.Succeeded)
                    {
                        //happy path
                        dto = mapper.Map<SignalDto>(signal);
                        return Ok(dto);
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

        [HttpPost("api/amphorae/{id}/signals/values")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Obsolete]
        public async Task<IActionResult> UploadSignal_value_old(string id, [FromBody] Dictionary<string, object> data)
        {
            var result = await amphoraeService.ReadAsync(User, id, true);
            if (result.Succeeded)
            {
                await signalService.WriteSignalAsync(User, result.Entity, data);
                return Ok();
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
        /// Get's the signals associated with an Amphora.
        /// </summary>
        /// <param name="id">Amphora Id</param>  
        /// <param name="data">Signal Values</param>  
        /// <param name="apiVersion">Only 'Nov-19'</param>  
        [HttpPost("api/amphorae/{id}/signals/values"), HttpHeader("X-Api-Version", "Nov-19")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UploadSignal_value([FromHeader(Name = "X-Api-Version")] string apiVersion, string id, SignalValuesDto data)
        {
            if (data?.SignalValues == null || data.SignalValues.Count == 0) return BadRequest();
            if (string.IsNullOrEmpty(apiVersion)) return BadRequest("X-Api-Version is a required header.");
            if (!ModelState.IsValid) return BadRequest();
            var result = await amphoraeService.ReadAsync(User, id, true);
            if (result.Succeeded)
            {
                await signalService.WriteSignalAsync(User, result.Entity, data.SignalValues.ToObjectDictionary());
                return Ok();
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
    }
}
