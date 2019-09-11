using Amphora.Common.Contracts;
using Amphora.Common.Extensions;

namespace Amphora.Common.Models
{
    public class Organisation : Entity, IEntity
    {
        // needs nothing as of now
        public string InviteCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string WebsiteUrl { get; set; }
        public string Address { get; set; }
        public override void SetIds()
        {
            this.OrganisationId = System.Guid.NewGuid().ToString();
            this.Id = this.OrganisationId.AsQualifiedId(typeof(Organisation));
        }

        // Address
        // Registration Number -- like ACN or ABN or something
        // billing thing here
        // current discount program / incentives...
    }
}