using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Common.Models.Signals;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Amphora.Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public partial class AmphoraeController : Controller
    {
        /// <summary>
        /// Get's the signals associated with an Amphora.
        /// </summary>
        /// <param name="id">Amphora Id</param>  
        [Produces(typeof(List<SignalDto>))]
        [HttpGet("api/amphorae/{id}/signals")]
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
        public async Task<IActionResult> CreateSignal(string id, [FromBody] SignalDto dto)
        {
            try
            {
                var signal = new SignalModel(dto.KeyName, dto.ValueType);
                var result = await amphoraeService.ReadAsync(User, id, true);
                if (result.Succeeded)
                {
                    result.Entity.AddSignal(signal);
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
        /// <summary>
        /// Get's the signals associated with an Amphora.
        /// </summary>
        /// <param name="id">Amphora Id</param>  
        /// <param name="data">Signal Values</param>  
        [HttpPost("api/amphorae/{id}/signals/values")]
        public async Task<IActionResult> UploadSignal_value(string id, [FromBody] Dictionary<string, object> data)
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
    }
}
