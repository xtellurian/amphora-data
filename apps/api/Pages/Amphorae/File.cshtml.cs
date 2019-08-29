using System.Threading.Tasks;
using Amphora.Api.Contracts;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Pages.Amphorae
{
    [Authorize]
    public class FileModel : PageModel
    {
        private readonly IOrgScopedEntityStore<Common.Models.Amphora> entityStore;
        private readonly IDataStore<Common.Models.Amphora, byte[]> dataStore;
        private readonly ITsiService tsiService;
        private readonly IMapper mapper;

        public FileModel(
            IOrgScopedEntityStore<Amphora.Common.Models.Amphora> entityStore,
            IDataStore<Amphora.Common.Models.Amphora, byte[]> dataStore,
            ITsiService tsiService,
            IMapper mapper)
        {
            this.entityStore = entityStore;
            this.dataStore = dataStore;
            this.tsiService = tsiService;
            this.mapper = mapper;
        }

        public async Task<IActionResult> OnGetAsync(string id, string name)
        {
            if(string.IsNullOrEmpty(name)) return RedirectToPage("./Detail", new {Id = id});
            var entity = await entityStore.ReadAsync(id);
            if (entity == null)
            {
                return RedirectToPage("/amphorae/index");
            }
            var file = await dataStore.GetDataAsync(entity, name);
            if(file == null || file.Length == 0) return RedirectToPage("./Detail", new {Id = id});
            return File(file, "application/octet-stream", name);
        }
    }
}