using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Extensions;
using Amphora.Api.Services.FeatureFlags;
using Amphora.Common.Contracts;
using Amphora.Common.Models;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Permissions;
using Amphora.Common.Models.UserData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using TimeSeriesInsightsClient.Queries;

namespace Amphora.Api.Pages.Amphorae
{
    [Authorize]
    public class DetailModel : PageModel
    {
        private readonly IAmphoraeService amphoraeService;
        private readonly IBlobStore<AmphoraModel> blobStore;
        private readonly IUserService userService;
        private readonly IPermissionService permissionService;
        private readonly FeatureFlagService featureFlags;
        private readonly ITsiService tsiService;

        public DetailModel(
            IAmphoraeService amphoraeService,
            IBlobStore<AmphoraModel> blobStore,
            IUserService userService,
            IPermissionService permissionService,
            FeatureFlagService featureFlags,
            ITsiService tsiService)
        {
            this.amphoraeService = amphoraeService;
            this.blobStore = blobStore;
            this.userService = userService;
            this.permissionService = permissionService;
            this.featureFlags = featureFlags;
            this.tsiService = tsiService;
        }

        [BindProperty]
        public AmphoraExtendedModel Amphora { get; set; }
        public IEnumerable<string> Names { get; set; }
        public Amphora.Common.Models.Domains.Domain Domain { get; set; }
        public string QueryResponse { get; set; }
        public bool CanEditPermissions { get; set; }
        public IEnumerable<IApplicationUserReference> HasPurchased { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return RedirectToPage("./Index");
            var result = await amphoraeService.ReadAsync<AmphoraExtendedModel>(User, id);
            var user = await userService.UserManager.GetUserAsync(User);
            if (result.WasForbidden)
            {
                return RedirectToPage("./Forbidden");
            }
            else if (result.Succeeded)
            {
                Amphora = result.Entity;
                if (Amphora == null)
                {
                    return RedirectToPage("./Index");
                }

                Names = await blobStore.ListBlobsAsync(Amphora);    
                Domain = Common.Models.Domains.Domain.GetDomain(Amphora.DomainId);
                QueryResponse = await GetQueryResponse();
                CanEditPermissions = await permissionService.IsAuthorizedAsync(user, this.Amphora, ResourcePermissions.Create);
                if(CanEditPermissions)
                {
                    var securityModel = await amphoraeService.AmphoraStore.ReadAsync<AmphoraSecurityModel>(Amphora.Id, Amphora.OrganisationId);
                    this.HasPurchased = securityModel.HasPurchased ?? new List<ApplicationUserReference>();;
                }
                return Page();
            }
            else
            {
                return RedirectToPage("./Index");
            }
        }

        public async Task<IActionResult> OnPostAsync(string id, List<IFormFile> files)
        {
            if (files == null || files.Count > 1)
            {
                throw new System.ArgumentException("Only 1 file is supported");
            }

            if (string.IsNullOrEmpty(id)) return RedirectToAction("./Index");

            var result = await amphoraeService.ReadAsync<AmphoraExtendedModel>(User, id);
            if (result.Succeeded)
            {
                if (result.Entity == null) return RedirectToPage("./Index");

                var formFile = files.FirstOrDefault();

                if (formFile != null && formFile.Length > 0)
                {
                    using (var stream = new MemoryStream())
                    {
                        await formFile.CopyToAsync(stream);
                        stream.Seek(0, SeekOrigin.Begin);
                        await this.blobStore.WriteBytesAsync(result.Entity, formFile.FileName, await stream.ReadFullyAsync());
                    }
                }
                this.Amphora = result.Entity;
                this.Names = await blobStore.ListBlobsAsync(result.Entity);
                this.Domain = Common.Models.Domains.Domain.GetDomain(Amphora.DomainId);
                return Page();
            }
            else if (result.WasForbidden)
            {
                return RedirectToPage("./Forbidden");
            }
            else
            {
                return RedirectToPage(".Index");
            }

        }

        private async Task<string> GetQueryResponse()
        {
            if (featureFlags.IsEnabled("signals"))
            {
                var response = new List<QueryResponse>();

                foreach (var member in Domain.GetDatumMembers())
                {
                    // then we can do a thing
                    if (string.Equals(member.Name, "t")) continue; // skip t // TODO remove hardcoding

                    var r = await tsiService.FullSet(
                            Amphora.Id,
                            member.Name,
                            DateTime.UtcNow.AddDays(-7),
                            DateTime.UtcNow
                        );

                    response.Add(r);
                }
                return JsonConvert.SerializeObject(response);
            }
            else return "";
        }
    }
}