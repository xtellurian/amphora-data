using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Amphora.Api.Controllers.Amphorae
{
    public partial class AmphoraeController : Controller
    {
        /// <summary>
        /// Get's a list of an Amphora's files
        /// </summary>
        /// <param name="id">Amphora Id</param>  
        [Produces(typeof(List<string>))]
        [HttpGet("api/amphorae/{id}/files")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
        /// <summary>
        /// Get's the contents of a file. Returns application/octet-stream
        /// </summary>
        /// <param name="id">Amphora Id</param>  
        /// <param name="file">The name of the file</param>  
        [HttpGet("api/amphorae/{id}/files/{file}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
        /// <summary>
        /// Set's the contents of a file. The request body becomes the content.
        /// </summary>
        /// <param name="id">Amphora Id</param>  
        /// <param name="file">The name of the file</param> 
        [HttpPut("api/amphorae/{id}/files/{file}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
