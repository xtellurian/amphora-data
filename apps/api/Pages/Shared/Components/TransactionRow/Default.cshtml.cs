using Amphora.Common.Models.Organisations.Accounts;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Pages.Shared.Components
{
    public class TransactionRowViewComponent : ViewComponent
    {
        public TransactionRowViewComponent()
        {
        }

        public Transaction Transaction { get; private set; }

        public IViewComponentResult Invoke(Transaction transaction, string size = "default")
        {
            this.Transaction = transaction;
            switch (size?.ToLower())
            {
                case "default":
                default:
                return View(this);
                case "medium":
                return View("Medium", this);
            }
        }
    }
}