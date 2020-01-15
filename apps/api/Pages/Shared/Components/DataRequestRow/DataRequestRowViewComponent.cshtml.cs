using Amphora.Common.Models.DataRequests;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Pages.Shared.Components
{
    [ViewComponent(Name = "DataRequestRow")]
    public class DataRequestRowViewComponent : ViewComponent
    {
        public DataRequestModel DataRequest { get; private set; }

        public IViewComponentResult Invoke(DataRequestModel dataRequest, bool isTable = false)
        {
            this.DataRequest = dataRequest;
            if (isTable)
            {
                return View("Table", this);
            }

            return View(this);
        }
    }
}