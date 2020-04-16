using Amphora.Common.Models.Permissions.Rules;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Pages.Shared.Components
{
    [ViewComponent(Name = "AccessRuleRow")]
    public class AccessRuleRowViewComponent : ViewComponent
    {
        public AccessRule Rule { get; private set; }
        public int Index { get; private set; }

        public IViewComponentResult Invoke(AccessRule rule, int index = -1)
        {
            this.Rule = rule;
            this.Index = index;
            return View(this);
        }
    }
}