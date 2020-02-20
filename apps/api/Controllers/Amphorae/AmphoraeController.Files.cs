using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Api.Extensions;
using Amphora.Api.Models.Dtos.Amphorae.Files;
using Amphora.Common.Models.Amphorae;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Amphora.Api.Controllers.Amphorae
{
    [ApiMajorVersion(0)]
    [ApiController]
    [SkipStatusCodePages]
    [Route("api/amphorae/{id}/files")]
    [OpenApiTag("Amphorae")]
    public class AmphoraeFilesController : EntityController
    {
        private readonly IAmphoraeService amphoraeService;
        private readonly IAmphoraFileService amphoraFileService;

        public AmphoraeFilesController(IAmphoraeService amphoraeService, IAmphoraFileService amphoraFileService)
        {
            this.amphoraeService = amphoraeService;
            this.amphoraFileService = amphoraFileService;
        }

        /// <summary>
        /// Get's a list of an Amphora's files.
        /// </summary>
        /// <param name="id">Amphora Id.</param>
        /// <returns>A list of file names.</returns>
        [Produces(typeof(List<string>))]
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ListFiles(string id)
        {
            var result = await amphoraeService.ReadAsync(User, id);
            if (result.Succeeded)
            {
                var blobs = await amphoraFileService.Store.ListBlobsAsync(result.Entity);
                return Ok(blobs);
            }
            else { return Handle(result); }
        }

        /// <summary>
        /// Get's the contents of a file. Returns application/octet-stream.
        /// </summary>
        /// <param name="id">Amphora Id.</param>
        /// <param name="file">The name of the file.</param>
        /// <returns>The file contents.</returns>
        [HttpGet("{file}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DownloadFile(string id, string file)
        {
            var result = await amphoraeService.ReadAsync(User, id);
            if (result.Succeeded)
            {
                var fileResult = await amphoraFileService.ReadFileAsync(User, result.Entity, file);
                if (fileResult.Succeeded && fileResult.Entity != null)
                {
                    var contentType = ContentTypeRecogniser.GetContentType(file);
                    return File(fileResult.Entity, contentType, file);
                }
                else { return Handle(fileResult); }
            }
            else { return Handle(result); }
        }

        /// <summary>
        /// Set's the contents of a file. The request body becomes the content.
        /// </summary>
        /// <param name="id">Amphora Id.</param>
        /// <param name="file">The name of the file.</param>
        /// <param name="content">The content of the file.</param>
        /// <returns>An object with a blob URL.</returns>
        [HttpPut("{file}")]
        [Produces(typeof(UploadResponse))]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [DisableRequestSizeLimit]
        [OpenApiIgnore]
        public async Task<IActionResult> UploadFile(string id, string file, [FromForm] IFormFile content)
        {
            var result = await amphoraeService.ReadAsync(User, id);
            if (result.Succeeded)
            {
                Models.EntityOperationResult<UploadResponse> fileResult;
                if (content != null && content.Length > 0)
                {
                    using (var stream = new MemoryStream())
                    {
                        await content.CopyToAsync(stream);
                        stream.Seek(0, SeekOrigin.Begin);
                        fileResult = await amphoraFileService.WriteFileAsync(User, result.Entity, await stream.ReadFullyAsync(), file);
                    }
                }
                else
                {
                    fileResult = await amphoraFileService.WriteFileAsync(User, result.Entity, await Request.Body.ReadFullyAsync(), file);
                }

                if (fileResult.Succeeded)
                {
                    return Ok(fileResult.Entity);
                }
                else { return Handle(fileResult); }
            }
            else { return Handle(result); }
        }

        /// <summary>
        /// Creates a file. Returns a blob URL to upload to.
        /// </summary>
        /// <param name="id">Amphora Id.</param>
        /// <param name="file">The name of the file.</param>
        /// <returns>An object with a blob URL.</returns>
        [HttpPost("{file}")]
        [Produces(typeof(UploadResponse))]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CreateFileRequest(string id, string file)
        {
            var result = await amphoraeService.ReadAsync(User, id);
            if (result.Succeeded)
            {
                var fileResult = await amphoraFileService.CreateFileAsync(User, result.Entity, file);

                if (fileResult.Succeeded)
                {
                    return Ok(fileResult.Entity);
                }
                else { return Handle(fileResult); }
            }
            else
            {
                return Handle(result);
            }
        }

        /// <summary>
        /// Get's a list of an Amphora's files.
        /// </summary>
        /// <param name="id">Amphora Id.</param>
        /// <param name="file">The name of the file.</param>
        /// <param name="metadata">A dict containing metadata for the file.</param>
        /// <returns>A list of file names.</returns>
        [Produces(typeof(Dictionary<string, string>))]
        [HttpPost("{file}/meta")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> WriteFileMetadata(string id, string file, [FromBody] Dictionary<string, string> metadata)
        {
            var result = await amphoraeService.ReadAsync(User, id);
            if (result.Succeeded)
            {
                var entity = result.Entity;
                var fileExists = await amphoraFileService.Store.ExistsAsync(result.Entity, file);
                if (fileExists)
                {
                    // the file exists, now update the metadata
                    entity.FileAttributes ??= new Dictionary<string, AttributeStore>();

                    entity.FileAttributes[file] = new AttributeStore(metadata);
                    var updateRes = await amphoraeService.UpdateAsync(User, entity);
                    if (updateRes.Succeeded)
                    {
                        return Ok(metadata);
                    }
                    else { return Handle(updateRes); }
                }
                else { return NotFound(); }
            }
            else { return Handle(result); }
        }
    }
}
