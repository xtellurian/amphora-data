using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Dtos;
using Amphora.Api.Models.Dtos.Applications;
using Amphora.Common.Models.Applications;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Amphora.Api.Controllers
{
    [ApiMajorVersion(0)]
    [ApiController]
    [SkipStatusCodePages]
    [Route("api/applications")]
    [OpenApiTag("Applications")]
    public class ApplicationsController : EntityController
    {
        private readonly IApplicationService applicationService;
        private readonly IMapper mapper;

        public ApplicationsController(IApplicationService applicationService, IMapper mapper)
        {
            this.applicationService = applicationService;
            this.mapper = mapper;
        }

        /// <summary>
        /// Creates a new application.
        /// Applications are external websites that Amphora users can sign in to.
        /// </summary>
        /// <param name="application">An application to create.</param>
        /// <returns>The Application information.</returns>
        [HttpPost]
        [CommonAuthorize]
        [Produces(typeof(Application))]
        [ProducesBadRequest]
        [ValidateModel]
        public async Task<IActionResult> CreateApplication(CreateApplication application)
        {
            var model = mapper.Map<ApplicationModel>(application);
            var result = await applicationService.CreateAsync(User, model);
            if (result.Succeeded)
            {
                var mapped = mapper.Map<Application>(result.Entity);
                return Ok(mapped);
            }
            else
            {
                return Handle(result);
            }
        }

        /// <summary>
        /// Gets an application by Id, if it exists.
        /// </summary>
        /// <param name="id">The id of the application to get.</param>
        /// <returns>The Application information.</returns>
        [HttpGet("{id}")]
        [CommonAuthorize]
        [Produces(typeof(Application))]
        [ValidateModel]
        public async Task<IActionResult> ReadApplication(string id)
        {
            var result = await applicationService.ReadAsync(User, id);
            if (result.Succeeded)
            {
                var mapped = mapper.Map<Application>(result.Entity);
                return Ok(mapped);
            }
            else
            {
                return Handle(result);
            }
        }

        /// <summary>
        /// Gets all applications a user has access to.
        /// </summary>
        /// <returns>The Application information.</returns>
        [HttpGet]
        [CommonAuthorize]
        [Produces(typeof(CollectionResponse<Application>))]
        [ValidateModel]
        public async Task<IActionResult> GetApplications()
        {
            var result = await applicationService.ListAsync(User);
            if (result.Succeeded)
            {
                var mapped = mapper.Map<List<Application>>(result.Entity);
                return Ok(new CollectionResponse<Application>(mapped));
            }
            else
            {
                return Handle(result);
            }
        }

        /// <summary>
        /// Updates an application by Id, if it exists.
        /// </summary>
        /// <param name="id">The id of the application to update.</param>
        /// <param name="application">The information to update (old locations will be deleted).</param>
        /// <returns>The Application information.</returns>
        [HttpPut("{id}")]
        [CommonAuthorize]
        [Produces(typeof(Application))]
        [ValidateModel]
        public async Task<IActionResult> UpdateApplication(string id, [FromBody] UpdateApplication application)
        {
            if (application?.Id != id)
            {
                return BadRequest("URL and model Id parameter must match.");
            }

            var result = await applicationService.ReadAsync(User, id);
            if (result.Succeeded)
            {
                // load locations so they are tracked.
                var model = result.Entity;
                var currentLocations = model.Locations;
                model.Name = application.Name;
                model.LogoutUrl = application.LogoutUrl;
                model.Locations = mapper.Map<List<ApplicationLocationModel>>(application.Locations);
                var res = await applicationService.UpdateAsync(User, model);
                if (res.Succeeded)
                {
                    return Ok(res.Entity);
                }
                else
                {
                    return Handle(res);
                }
            }
            else
            {
                return Handle(result);
            }
        }

        /// <summary>
        /// Deletes an application.
        /// Must be done by an Organisation administrator.
        /// </summary>
        /// <param name="id">The application id to delete.</param>
        /// <returns>200 if successful.</returns>
        [HttpDelete("{id}")]
        [CommonAuthorize]
        [Produces(typeof(Application))]
        [ValidateModel]
        public async Task<IActionResult> DeleteApplication(string id)
        {
            var result = await applicationService.DeleteAsync(User, id);
            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                return Handle(result);
            }
        }
    }
}