using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Extensions;
using Amphora.Api.Services.FeatureFlags;
using Amphora.Common.Models;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Permissions;
using Amphora.Common.Models.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TimeSeriesInsightsClient.Queries;

namespace Amphora.Api.Pages.Amphorae
{
    [Authorize]
    public class DetailModel : AmphoraPageModel
    {
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
            ITsiService tsiService) : base(amphoraeService)
        {
            this.blobStore = blobStore;
            this.userService = userService;
            this.permissionService = permissionService;
            this.featureFlags = featureFlags;
            this.tsiService = tsiService;
        }

        public IEnumerable<string> Names { get; set; }
        public Amphora.Common.Models.Domains.Domain Domain { get; set; }
        public string QueryResponse { get; set; }
        public bool CanEditPermissions { get; set; }
        public bool CanEditDetails { get; private set; }
        public bool CanUploadFiles { get; private set; }
        public IEnumerable<TransactionModel> Transactions { get; private set; }
        public bool CanBuy { get; set; }

        public override async Task<IActionResult> OnGetAsync(string id)
        {
            var response = await base.OnGetAsync(id);
            await SetPagePropertiesAsync();

            return response;
        }

        private async Task SetPagePropertiesAsync()
        {
            var user = await userService.UserManager.GetUserAsync(User);
            if (Amphora != null)
            {
                Names = await blobStore.ListBlobsAsync(Amphora);
                Domain = Common.Models.Domains.Domain.GetDomain(Amphora.DomainId);
                QueryResponse = await GetQueryResponse();
                CanEditPermissions = await permissionService.IsAuthorizedAsync(user, this.Amphora, ResourcePermissions.Create);
                // can edit permissions implies can edit details - else, check
                CanEditDetails = CanEditPermissions ? true : await permissionService.IsAuthorizedAsync(user, this.Amphora, ResourcePermissions.Update);
                CanUploadFiles = CanEditDetails ? true : await permissionService.IsAuthorizedAsync(user, this.Amphora, ResourcePermissions.WriteContents);

               
                this.Transactions = Amphora?.Transactions ?? new List<TransactionModel>();
                if (this.Transactions?.Any(u => string.Equals(u.Id, user.Id)) ?? false)
                {
                    // user has already purchased the amphora
                    this.CanBuy = false;
                }
                else
                {
                    this.CanBuy = true;
                }
            }
        }

        public async Task<IActionResult> OnPostAsync(string id, List<IFormFile> files)
        {
            if (files == null || files.Count > 1)
            {
                throw new System.ArgumentException("Only 1 file is supported");
            }

            if (string.IsNullOrEmpty(id)) return RedirectToAction("./Index");

            var result = await amphoraeService.ReadAsync(User, id);
            var user = await userService.UserManager.GetUserAsync(User);

            if (result.Succeeded)
            {
                if (result.Entity == null) return RedirectToPage("./Index");

                if (await permissionService.IsAuthorizedAsync(user, result.Entity, AccessLevels.WriteContents))
                {
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
                }
                else
                {
                    return RedirectToPage("./Forbidden");
                }
                this.Amphora = result.Entity;
                await SetPagePropertiesAsync();
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