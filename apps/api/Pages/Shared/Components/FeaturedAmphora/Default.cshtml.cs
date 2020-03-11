using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Options;
using Amphora.Common.Models.Amphorae;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Amphora.Api.Pages.Shared.Components.FeaturedAmphora
{
    public class FeaturedAmphoraViewComponent : ViewComponent
    {
        private readonly IAmphoraeService amphoraeService;
        private readonly IOptionsMonitor<AmphoraManagementOptions> options;

        public FeaturedAmphoraViewComponent(IAmphoraeService amphoraeService,
                                            IOptionsMonitor<AmphoraManagementOptions> options)
        {
            this.amphoraeService = amphoraeService;
            this.options = options;
        }

        public AmphoraModel Amphora { get; private set; }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (!string.IsNullOrEmpty(options.CurrentValue?.FeaturedAmphoraId))
            {
                this.Amphora = await amphoraeService.AmphoraStore.ReadAsync(options.CurrentValue.FeaturedAmphoraId);
            }

            return View(this);
        }
    }
}