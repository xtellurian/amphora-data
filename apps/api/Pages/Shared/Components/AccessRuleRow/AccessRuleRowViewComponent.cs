using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Permissions.Rules;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Pages.Shared.Components
{
    [ViewComponent(Name = "AccessRuleRow")]
    public class AccessRuleRowViewComponent : ViewComponent
    {
        public AccessRule Rule { get; private set; }
        public AmphoraModel Amphora { get; private set; }
        public int Index { get; private set; }

        public IViewComponentResult Invoke(AccessRule rule, AmphoraModel amphora, int index = -1)
        {
            this.Rule = rule;
            this.Amphora = amphora;
            this.Index = index;
            return View(this);
        }
    }
}