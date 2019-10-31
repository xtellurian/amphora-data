using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Signals;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Areas.Amphorae.Pages.Signals
{
    [Authorize]
    public class IndexModel : AmphoraPageModel
    {
        public IndexModel(IAmphoraeService amphoraeService) : base(amphoraeService)
        {
        }
        public IEnumerable<SignalModel> Signals { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            await base.LoadAmphoraAsync(id);

            if (Amphora != null)
            {
                this.Signals = this.Amphora.Signals.Select(s => s.Signal);
            }

            return OnReturnPage();

        }
    }
}


