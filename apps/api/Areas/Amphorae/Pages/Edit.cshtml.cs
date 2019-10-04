using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Common.Models.Amphorae;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Areas.Amphorae.Pages
{
    public class EditModel : AmphoraPageModel
    {
        private readonly IMapper mapper;

        public EditModel(IAmphoraeService amphoraeService, IMapper mapper): base(amphoraeService)
        {
            this.mapper = mapper;
        }

        [BindProperty]
        public AmphoraExtendedDto AmphoraDto { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            await base.LoadAmphoraAsync(id);
            if(Amphora != null)
            {
                this.AmphoraDto = mapper.Map<AmphoraExtendedDto>(Amphora);
            }
           return OnReturnPage();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            var readResult = await amphoraeService.ReadAsync(User, id);
            if(readResult.Succeeded)
            {
                var a = readResult.Entity;
                a.Name = AmphoraDto.Name;
                a.Description = AmphoraDto.Description;
                a.Price = AmphoraDto.Price;
                a.GeoLocation = (AmphoraDto.Lon.HasValue && AmphoraDto.Lat.HasValue) 
                    ? new GeoLocation(AmphoraDto.Lon.Value, AmphoraDto.Lat.Value) 
                    : null;
                
                var result = await amphoraeService.UpdateAsync(User, a);
                if(result.Succeeded)
                {
                    return RedirectToPage("./Detail", new {id = a.Id});
                }
                else if(result.WasForbidden)
                {
                    return RedirectToPage("./Forbidden");
                }
                else
                {
                    this.ModelState.AddModelError(string.Empty, result.Message);
                    return Page();
                }
            }
            else if(readResult.WasForbidden)
            {
                return RedirectToPage("./Forbidden");
            }
            else
            {
                this.ModelState.AddModelError(string.Empty, readResult.Message);
                return Page();
            }
        }
    }
}