using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                    var updateRes = await signalService.AddSignal(User, result.Entity, signal);

                    if (updateRes.Succeeded)
                    {
                        //happy path
                        dto = mapper.Map<SignalDto>(updateRes.Entity);
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

        /// <summary>
        /// Uploads values to an Amphora signal(s)
        /// </summary>
        /// <param name="id">Amphora Id</param>  
        /// <param name="data">Signal Values</param>  
        [HttpPost("api/amphorae/{id}/signals/values")]
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
                else return BadRequest(res.Message);
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
        /// Uploads values in batch to an Amphora signal(s)
        /// </summary>
        /// <param name="id">Amphora Id</param>  
        /// <param name="data">Signal Values</param>  
        [HttpPost("api/amphorae/{id}/signals/batchvalues")]
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
                else return BadRequest(res.Message);
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
