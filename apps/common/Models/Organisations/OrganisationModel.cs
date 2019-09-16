using Amphora.Common.Contracts;
using Amphora.Common.Extensions;

namespace Amphora.Common.Models.Organisations
{
    public class OrganisationModel : Entity, IEntity
    {
        public string Name { get; set; }
        
        public override void SetIds()
        {
            this.OrganisationId = System.Guid.NewGuid().ToString();
            this.Id = this.OrganisationId.AsQualifiedId(typeof(OrganisationModel));
            this.EntityType = typeof(OrganisationModel).GetEntityPrefix();
        }

        // Address
        // Registration Number -- like ACN or ABN or something
        // billing thing here
        // current discount program / incentives...
    }
}