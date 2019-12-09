using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Common.Models.Organisations.Accounts;

namespace Amphora.Api.Contracts
{
    public interface IAccountsService
    {
        Task<IEnumerable<Invoice>> GenerateInvoicesAsync(DateTimeOffset month, bool regenerate = false);
        Task PopulateDebitsAsync();
    }
}