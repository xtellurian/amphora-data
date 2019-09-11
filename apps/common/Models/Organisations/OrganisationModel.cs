using Amphora.Common.Contracts;
using Amphora.Common.Extensions;

namespace Amphora.Common.Models.Organisations
{
    public class OrganisationModel : Entity, IEntity
    {
        // needs nothing as of now
        public string InviteCode { get; set; }
        public string Name { get; set; }
        public string About { get; set; }
        public string WebsiteUrl { get; set; }
        public string Address { get; set; }
        public override void SetIds()
        {
            this.OrganisationId = System.Guid.NewGuid().ToString();
            this.Id = this.OrganisationId.AsQualifiedId(typeof(OrganisationModel));
        }

        // Address
        // Registration Number -- like ACN or ABN or something
        // billing thing here
        // current discount program / incentives...
    }
}