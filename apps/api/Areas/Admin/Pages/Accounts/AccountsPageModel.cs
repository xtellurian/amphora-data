using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Organisations;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Amphora.Api.Areas.Admin.Pages.Accounts
{
    public abstract class AccountsPageModel : PageModel
    {
        protected readonly IEntityStore<OrganisationModel> orgStore;

        protected AccountsPageModel(IEntityStore<OrganisationModel> orgStore)
        {
            this.orgStore = orgStore;
        }

        public List<OrganisationModel> Orgs { get; private set; } = new List<OrganisationModel>();
        public int OrgCount { get; private set; }
        public string Name { get; private set; }
        public int PageNumber { get; private set; }
        public int PerPage { get; private set; }
        public int MaxPages { get; private set; }

        protected async Task LoadOrgsAsync(int pageNumber, int perPage, string name)
        {
            OrgCount = await orgStore.CountAsync();
            this.Name = name;
            this.PageNumber = pageNumber;
            this.PerPage = perPage;
            this.MaxPages = 1 + (OrgCount / perPage);
            if (name == null)
            {
                Orgs = await orgStore.Query(_ => true)
                    .Skip(pageNumber * perPage)
                    .Take(perPage)
                    .ToListAsync();
            }
            else
            {
                Orgs = (await orgStore.QueryAsync(_ => true))
                    .Where(_ => _.Name.ToLower().Contains(name))
                    .Skip(pageNumber * perPage)
                    .Take(perPage)
                    .ToList();
            }
        }
    }
}