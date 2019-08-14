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

namespace Amphora.Api.Pages.Amphorae
{
    [Authorize]
    public class DetailModel : PageModel
    {
        private readonly IOrgScopedEntityStore<Common.Models.Amphora> amphoraEntityStore;
        private readonly IDataStore<Common.Models.Amphora, byte[]> dataStore;
        private readonly IMapper mapper;

        public DetailModel(
            IOrgScopedEntityStore<Amphora.Common.Models.Amphora> amphoraEntityStore,
            IDataStore<Amphora.Common.Models.Amphora, byte[]> dataStore,
            IMapper mapper)
        {
            this.amphoraEntityStore = amphoraEntityStore;
            this.dataStore = dataStore;
            this.mapper = mapper;
        }

        [BindProperty]
        public Amphora.Common.Models.Amphora Amphora { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            Amphora = await amphoraEntityStore.ReadAsync(id);
            if(Amphora == null)
            {
                return RedirectToPage("/amphorae/index");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id,  List<IFormFile> files)
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
                    await this.dataStore.SetDataAsync(entity, await stream.ReadFullyAsync());
                }
                entity.FileName = formFile.FileName;
                await amphoraEntityStore.UpdateAsync(entity);
            }
            this.Amphora = entity;

            return Page();
        }

    }
}