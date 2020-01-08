using Amphora.Api.Contracts;
using Amphora.Api.DbContexts;
using Amphora.Common.Models.Organisations;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Stores.EFCore
{
    public class OrganisationsEFStore : EFStoreBase<OrganisationModel>, IEntityStore<OrganisationModel>
    {
        public OrganisationsEFStore(AmphoraContext context, ILogger<OrganisationsEFStore> logger) : base(context, logger, db => db.Organisations)
        {
        }
    }
}