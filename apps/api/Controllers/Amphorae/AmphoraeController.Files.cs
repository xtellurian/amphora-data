using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Api.Extensions;
using Amphora.Api.Models.Dtos.Amphorae.Files;
using Amphora.Common.Contracts;
using Amphora.Common.Models;
using Amphora.Common.Models.Amphorae;
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
        /// Lists an Amphora's files.
        /// </summary>
        /// <param name="id">Amphora Id.</param>
        /// <param name="options">Option for listing the files.</param>
        /// <returns>A list of file names.</returns>
        [Produces(typeof(List<string>))]
        [HttpGet]
        [CommonAuthorize]
        public async Task<IActionResult> ListFiles(string id, [FromQuery] FileListOptions options)
        {
            var result = await amphoraeService.ReadAsync(User, id);
            options ??= FileListOptions.Default;
            if (result.Succeeded)
            {
                var files = await ListFilesAsync(result.Entity, options);
                var names = files.Select(_ => _.Name);
                return Ok(names);
            }
            else { return Handle(result); }
        }

        private async Task<IList<IAmphoraFileReference>> ListFilesAsync(AmphoraModel model, FileListOptions options)
        {
            var files = await amphoraFileService.Store.GetFilesAsync(model, options.Prefix, options.Skip ?? 0, options.Take ?? 64);
            switch (options.OrderBy)
            {
                case FileQueryOptions.Alphabetical:
                    files = files.OrderBy(_ => _.Name).ToList();
                    break;
                case FileQueryOptions.LastModified:
                    files = files.OrderBy(_ => _.LastModified).ToList();
                    break;
            }

            return files;
        }

        /// <summary>
        /// Queries an Amphora's files.
        /// </summary>
        /// <param name="id">Amphora Id.</param>
        /// <param name="options">Option for querying the files.</param>
        /// <returns>A list of file names.</returns>
        [Produces(typeof(List<string>))]
        [HttpPost]
        [CommonAuthorize]
        public async Task<IActionResult> QueryFiles(string id, FileQueryOptions options)
        {
            var result = await amphoraeService.ReadAsync(User, id);
            options ??= FileQueryOptions.Default;
            if (result.Succeeded)
            {
                var files = await ListFilesAsync(result.Entity, options);

                var final = new List<IAmphoraFileReference>();
                if (options.Attributes != null && options.Attributes.Count > 0)
                {
                    if (options.AllAttributes)
                    {
                        final.AddRange(files
                            .Where(file => options.Attributes
                                .All(optionAttribute =>
                                    file.Metadata.ContainsKey(optionAttribute.Key) &&
                                    file.Metadata[optionAttribute.Key] == optionAttribute.Value)));
                    }
                    else
                    {
                        final.AddRange(files
                            .Where(file => options.Attributes
                                .Any(optionAttribute =>
                                    file.Metadata.ContainsKey(optionAttribute.Key) &&
                                    file.Metadata[optionAttribute.Key] == optionAttribute.Value)));
                    }
                }
                else
                {
                    final.AddRange(files);
                }

                var names = final.Select(_ => _.Name);
                return Ok(names);
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
        [ProducesResponseType(typeof(FileResult), 200)]
        [CommonAuthorize]
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
        [CommonAuthorize]
        [DisableRequestSizeLimit]
        [OpenApiIgnore]
        public async Task<IActionResult> UploadFile(string id, string file, [FromForm] IFormFile content)
        {
            var result = await amphoraeService.ReadAsync(User, id);
            if (result.Succeeded)
            {
                EntityOperationResult<UploadResponse> fileResult;
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
        [CommonAuthorize]
        public async Task<IActionResult> CreateFileRequest(string id, string file)
        {
            var result = await amphoraeService.ReadAsync(User, id);
            if (result.Succeeded)
            {
                var fileResult = await amphoraFileService.CreateFileAsync(User, result.Entity, file);

                if (fileResult.Succeeded)
                {
                    // then it's relative, add the host here.
                    if (fileResult.Entity.Url.StartsWith('/'))
                    {
                        fileResult.Entity.Url = $"{Request.Scheme}://{Request.Host}{fileResult.Entity.Url}";
                    }

                    return Ok(fileResult.Entity);
                }
                else
                {
                    return Handle(fileResult);
                }
            }
            else
            {
                return Handle(result);
            }
        }

        /// <summary>
        /// Gets the attributes of a file.
        /// </summary>
        /// <param name="id">Amphora Id.</param>
        /// <param name="file">The name of the file.</param>
        /// <param name="attributes">A dict containing attributes for the file.</param>
        /// <returns>The attributes.</returns>
        [Produces(typeof(Dictionary<string, string>))]
        [HttpPost("{file}/attributes")]
        [CommonAuthorize]
        public async Task<IActionResult> WriteFileAttributes(string id, string file, [FromBody] Dictionary<string, string> attributes)
        {
            var result = await amphoraeService.ReadAsync(User, id);
            if (result.Succeeded)
            {
                var entity = result.Entity;
                var fileExists = await amphoraFileService.Store.ExistsAsync(result.Entity, file);
                if (fileExists)
                {
                    // the file exists, now update the metadata
                    var writeResult = await amphoraFileService.WriteAttributesAsync(User, entity, attributes, file);
                    if (writeResult.Succeeded)
                    {
                        return Ok(attributes);
                    }
                    else { return Handle(writeResult); }
                }
                else { return NotFound(); }
            }
            else { return Handle(result); }
        }

        /// <summary>
        /// Get's the attributes of a file.
        /// </summary>
        /// <param name="id">Amphora Id.</param>
        /// <param name="file">The name of the file.</param>
        /// <returns>The attributes.</returns>
        [Produces(typeof(Dictionary<string, string>))]
        [HttpGet("{file}/attributes")]
        [CommonAuthorize]
        public async Task<IActionResult> ReadFileAttributes(string id, string file)
        {
            var result = await amphoraeService.ReadAsync(User, id);
            if (result.Succeeded)
            {
                var entity = result.Entity;
                var fileExists = await amphoraFileService.Store.ExistsAsync(result.Entity, file);
                if (fileExists)
                {
                    // the file exists, now update the metadata
                    var readRes = await amphoraFileService.ReadAttributesAsync(User, entity, file);
                    if (readRes.Succeeded)
                    {
                        return Ok(readRes.Entity);
                    }
                    else { return Handle(readRes); }
                }
                else { return NotFound(); }
            }
            else { return Handle(result); }
        }

        /// <summary>
        /// Get's the contents of a file. Returns application/octet-stream.
        /// </summary>
        /// <param name="id">Amphora Id.</param>
        /// <param name="file">The name of the file.</param>
        /// <returns>200 if successful.</returns>
        [HttpDelete("{file}")]
        [CommonAuthorize]
        public async Task<IActionResult> DeleteFile(string id, string file)
        {
            var result = await amphoraeService.ReadAsync(User, id);
            if (result.Succeeded)
            {
                var deleteRes = await amphoraFileService.DeleteFileAsync(User, result.Entity, file);
                if (deleteRes.Succeeded)
                {
                    return Ok();
                }
                else
                {
                    return Handle(deleteRes);
                }
            }
            else { return Handle(result); }
        }
    }
}
