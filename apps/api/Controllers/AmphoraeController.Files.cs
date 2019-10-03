using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Extensions;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Common.Models.Amphorae;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Amphora.Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public partial class AmphoraeController : Controller
    {

        [HttpGet("api/amphorae/{id}/files")]
        public async Task<IActionResult> ListFiles(string id)
        {
            var result = await amphoraeService.ReadAsync(User, id);
            if (result.Succeeded)
            {
                var blobs = await amphoraFileService.Store.ListBlobsAsync(result.Entity);
                return Ok(blobs);
            }
            else if (result.WasForbidden)
            {
                return StatusCode(403, result.Message);
            }
            else
            {
                return NotFound("Amphora not found");
            }
        }

        [HttpGet("api/amphorae/{id}/files/{file}")]
        public async Task<IActionResult> DownloadFile(string id, string file)
        {
            var result = await amphoraeService.ReadAsync(User, id);
            if (result.Succeeded)
            {
                var fileResult = await amphoraFileService.ReadFileAsync(User, result.Entity, file);
                if (fileResult.Succeeded)
                {
                    return File(fileResult.Entity, "application/octet-stream", file);
                }
                else if (fileResult.WasForbidden)
                {
                    return StatusCode(403, fileResult.Message);
                }
                else
                {
                    return BadRequest(fileResult.Message);
                }
            }
            else if (result.WasForbidden)
            {
                return StatusCode(403, result.Message);
            }
            else
            {
                return NotFound("Amphora not found");
            }
        }

        [HttpPut("api/amphorae/{id}/files/{file}")]
        public async Task<IActionResult> UploadToAmphora(string id, string file)
        {
            var result = await amphoraeService.ReadAsync(User, id);
            if (result.Succeeded)
            {
                var fileResult = await amphoraFileService.WriteFileAsync(User, result.Entity, await Request.Body.ReadFullyAsync(), file);
                if (fileResult.Succeeded)
                {
                    return Ok();
                }
                else if (result.WasForbidden)
                {
                    return StatusCode(403, result.Errors);
                }
                else
                {
                    return NotFound();
                }
            }
            else if (result.WasForbidden)
            {
                return StatusCode(403, result.Errors);
            }
            else
            {
                return NotFound();
            }
        }


    }
}
