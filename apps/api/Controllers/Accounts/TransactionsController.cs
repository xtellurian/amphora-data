using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Dtos;
using Amphora.Api.Models.Dtos.Accounts;
using Amphora.Common.Contracts;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Amphora.Api.Controllers.Accounts
{
    [ApiController]
    [SkipStatusCodePages]
    [OpenApiTag("Account")]
    [Route("api/Account/Transactions")]
    public class TransactionsController : AccountControllerBase
    {
        private readonly IOrganisationService organisationService;
        private readonly IDateTimeProvider dtProvider;
        private readonly IMapper mapper;

        public TransactionsController(IOrganisationService organisationService,
                                      IUserDataService userDataService,
                                      IDateTimeProvider dtProvider,
                                      IMapper mapper) : base(userDataService)
        {
            this.organisationService = organisationService;
            this.dtProvider = dtProvider;
            this.mapper = mapper;
        }

        /// <summary>
        /// Gets the most recent transactions of the account.
        /// Defaults to the first 50 debits and 50 credits.
        /// </summary>
        /// <param name="id">Organisation Id.</param>
        /// <returns>A collection of recent transactions. </returns>
        [Produces(typeof(CollectionResponse<Transaction>))]
        [ProducesBadRequest]
        [HttpGet]
        [CommonAuthorize]
        public async Task<IActionResult> GetTransactions(string id = null)
        {
            var ensureRes = await EnsureIdAsync(id);
            if (ensureRes != null)
            {
                return ensureRes;
            }

            var orgRead = await organisationService.ReadAsync(User, OrganisationId);
            if (orgRead.Succeeded)
            {
                var org = orgRead.Entity;
                if (org.IsAdministrator(orgRead.User))
                {
                    var debits = org.Account.Debits.OrderBy(_ => _.Timestamp).Take(50);
                    var credits = org.Account.Credits.OrderBy(_ => _.Timestamp).Take(50);
                    var allTransactions = new List<Common.Models.Organisations.Accounts.Transaction>();
                    allTransactions.AddRange(credits);
                    allTransactions.AddRange(debits);
                    var tx = mapper.Map<List<Transaction>>(allTransactions);
                    return Ok(new CollectionResponse<Transaction>(tx));
                }
                else
                {
                    return BadRequest(new Response("You must be an administrator to view Transactions"));
                }
            }
            else
            {
                return Handle(orgRead);
            }
        }
    }
}