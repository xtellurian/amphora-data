using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NGeoHash;

namespace Amphora.Api.Pages.Amphorae
{
    [ValidateAntiForgeryToken]
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly IOrgScopedEntityStore<Common.Models.Amphora> amphoraEntityStore;
        private readonly IMapper mapper;

        public CreateModel(
            IOrgScopedEntityStore<Amphora.Common.Models.Amphora> amphoraEntityStore,
            IMapper mapper)
        {
            this.amphoraEntityStore = amphoraEntityStore;
            this.mapper = mapper;
        }

        public class InputModel
        {
            [Required]
            public string Title {get; set; }
            [Required]
            [DataType(DataType.MultilineText)]
            public string Description {get; set; }
            [Required]
            [DataType(DataType.Currency)]
            public double Price {get; set; }

            [Display(Name = "Latitude")]
            public double? Lat {get;set ;}
            [Display(Name = "Longitude")]
            public double? Lon {get;set ;}
        }
        [BindProperty]
        public InputModel Input {get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                string geoHash = null;
                if(Input.Lat.HasValue && Input.Lon.HasValue)
                {
                    geoHash = GeoHash.Encode(Input.Lat.Value, Input.Lon.Value);
                }
                var entity = new Amphora.Common.Models.Amphora
                {
                    Title = Input.Title,
                    Description = Input.Description,
                    GeoHash = geoHash,
                    Price = Input.Price,
                };

                var setResult = await amphoraEntityStore.CreateAsync(entity);
                return RedirectToPage("/Amphorae/Index");
            }
            else
            {
                return Page();
            }
        }
    }
}