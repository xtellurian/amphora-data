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
        [HttpGet("api/amphorae/{id}/signals")]
        public async Task<IActionResult> GetSignals(string id)
        {
            var result = await amphoraeService.ReadAsync(User, id, true);
            if(result.Succeeded)
            {
                var signals = result.Entity.Signals.Select(s => s.Signal);
                var res = mapper.Map<List<SignalDto>>(signals);
                return Ok(signals);
            }
            else if(result.WasForbidden)
            {
                return StatusCode(403, result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }

        [HttpPost("api/amphorae/{id}/signals")]
        public async Task<IActionResult> CreateSignal(string id, [FromBody] SignalDto dto)
        {
            try
            {
                var signal = new SignalModel(dto.KeyName, dto.ValueType);
                var result = await amphoraeService.ReadAsync(User, id, true);
                if(result.Succeeded)
                {
                    result.Entity.AddSignal(signal);
                    var updateRes = await amphoraeService.UpdateAsync(User, result.Entity);
                    if(updateRes.Succeeded)
                    {
                        //happy path
                        dto = mapper.Map<SignalDto>(signal);
                        return Ok(dto);
                    }
                    else if(result.WasForbidden)
                    {
                        return StatusCode(403, result.Message);
                    }
                    else
                    {
                        return BadRequest(result.Message);
                    }
                }
                else if(result.WasForbidden)
                {
                    return StatusCode(403, result.Message);
                }
                else
                {
                    return BadRequest(result.Message);
                }
            }
            catch(System.ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("api/amphorae/{id}/signals/{signalId}")]
        public async Task<IActionResult> UploadSignal_value(string id, string signalId, [FromBody] JObject jObj)
        {
            var result = await amphoraeService.ReadAsync(User, id, true);
            if (result.Succeeded)
            {
                var signal = result.Entity.Signals.FirstOrDefault(s => s.SignalId == signalId)?.Signal; 
                if(signal == null)
                {
                    return NotFound($"SignalId {signalId} not found on Amphora {id}");
                }
                await signalService.WriteSignalAsync(result.Entity, jObj);
                return Ok();
            }
            else if(result.WasForbidden)
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
