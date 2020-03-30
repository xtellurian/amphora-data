using System.Collections.Generic;
using Amphora.SharedUI.Models;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.SharedUI.Pages.Shared
{
    [ViewComponent(Name = "MasterNavigationPane")]
    public class MasterNavigationPaneViewComponent : ViewComponent
    {
        public MasterNavigationPaneViewModel.Page Index { get; private set; }
        public List<MasterNavigationPaneViewModel.Page> Pages { get; private set; }

        public IViewComponentResult Invoke(MasterNavigationPaneViewModel model)
        {
            this.Index = model.Index;
            this.Pages = model.Pages;

            if (IsPageActive(Index))
            {
                Index.IsActive = true;
            }
            else
            {
                foreach (var p in Pages)
                {
                    if (IsPageActive(p))
                    {
                        p.IsActive = true;
                    }
                }
            }

            return View(this);
        }

        private bool IsPageActive(MasterNavigationPaneViewModel.Page page)
        {
            var path = ViewContext.RouteData.Values["page"]?.ToString().TrimStart('.');
            return path.Contains(page.Path.TrimStart('.'));
        }
    }
}