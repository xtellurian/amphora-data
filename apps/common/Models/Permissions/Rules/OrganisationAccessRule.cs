using Amphora.Common.Models.Organisations;

namespace Amphora.Common.Models.Permissions.Rules
{
    public class OrganisationAccessRule : AccessRule
    {
        public OrganisationAccessRule() : base()
        { }

        public OrganisationAccessRule(Kind? kind, int? priority, OrganisationModel organisation)
        : base(kind, priority)
        {
            OrganisationId = organisation.Id;
            Organisation = organisation;
        }

        public string? OrganisationId { get; set; }
        public virtual OrganisationModel? Organisation { get; set; }

        public override string Name()
        {
            return $"{this.Kind} Organisation {this.Organisation?.Name}";
        }
    }
}