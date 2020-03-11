using Amphora.Api.Contracts;
using Amphora.Api.EntityFramework;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Stores.EFCore;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Stores.EFCore
{
    public class OrganisationsEFStore : EFStoreBase<OrganisationModel, AmphoraContext>, IEntityStore<OrganisationModel>
    {
        public OrganisationsEFStore(AmphoraContext context, ILogger<OrganisationsEFStore> logger) : base(context, logger, db => db.Organisations)
        {
        }
    }
}