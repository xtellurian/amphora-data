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

        public IViewComponentResult Invoke(Transaction transaction)
        {
            this.Transaction = transaction;
            return View(this);
        }
    }
}