using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Extensions;
using Amphora.Common.Models;
using AutoMapper;
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
        private readonly IEntityStore<Common.Models.Amphora> amphoraEntityStore;
        private readonly IDataStore<Common.Models.Amphora, byte[]> dataStore;
        private readonly ITsiService tsiService;
        private readonly IMapper mapper;

        public DetailModel(
            IEntityStore<Amphora.Common.Models.Amphora> amphoraEntityStore,
            IDataStore<Amphora.Common.Models.Amphora, byte[]> dataStore,
            ITsiService tsiService,
            IMapper mapper)
        {
            this.amphoraEntityStore = amphoraEntityStore;
            this.dataStore = dataStore;
            this.tsiService = tsiService;
            this.mapper = mapper;
        }

        [BindProperty]
        public Amphora.Common.Models.Amphora Amphora { get; set; }
        public IEnumerable<string> Names { get; set; }
        public Amphora.Common.Models.Domains.Domain Domain { get; set; }
        public string QueryResponse {get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            Amphora = await amphoraEntityStore.ReadAsync(id);
            if (Amphora == null)
            {
                return RedirectToPage("/amphorae/index");
            }
            Names = await dataStore.ListNamesAsync(Amphora);
            Domain = Common.Models.Domains.Domain.GetDomain(Amphora.DomainId);
            QueryResponse = await GetQueryResponse();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id, List<IFormFile> files)
        {
            if (files == null || files.Count > 1)
            {
                throw new System.ArgumentException("Only 1 file is supported");
            }

            if (string.IsNullOrEmpty(id)) return RedirectToAction("/Amphorae/Index");
            var entity = await amphoraEntityStore.ReadAsync(id);
            if (entity == null) return RedirectToPage("/Amphorae/Index");

            var formFile = files.FirstOrDefault();

            if (formFile != null && formFile.Length > 0)
            {
                using (var stream = new MemoryStream())
                {
                    await formFile.CopyToAsync(stream);
                    stream.Seek(0, SeekOrigin.Begin);
                    await this.dataStore.SetDataAsync(entity, await stream.ReadFullyAsync(), formFile.FileName);
                }
            }
            this.Amphora = entity;
            this.Names = await dataStore.ListNamesAsync(entity);
            this.Domain = Common.Models.Domains.Domain.GetDomain(Amphora.DomainId);
            return Page();
        }

        private async Task<string> GetQueryResponse()
        {
            var response = new List<QueryResponse>();
            foreach(var member in Domain.GetDatumMembers())
            {
                // then we can do a thing
                if(string.Equals(member.Name, "t")) continue; // skip t // TODO remove hardcoding
                // var r = await tsiService
                //     .WeeklyAverageAsync(
                //         Amphora.Id, 
                //         member.Name, 
                //         DateTime.UtcNow.AddDays(-365),
                //         DateTime.UtcNow
                //     );
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
    }
}