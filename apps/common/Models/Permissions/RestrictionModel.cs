using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Organisations;

namespace Amphora.Common.Models.Permissions
{
    /// <summary>
    /// Custom access restrictions to data in Amphora.
    /// </summary>
    public class RestrictionModel : EntityBase
    {
        public RestrictionModel()
        {
            TargetOrganisationId = null!;
            TargetOrganisation = null!;
        }

        public RestrictionModel(OrganisationModel sourceOrganisation, OrganisationModel targetOrganisation, RestrictionKind? kind = RestrictionKind.Deny)
        {
            TargetOrganisationId = targetOrganisation.Id;
            TargetOrganisation = targetOrganisation;
            SourceOrganisation = sourceOrganisation;
            SourceOrganisationId = sourceOrganisation.Id;
            Kind = kind;
            Scope = RestrictionScope.Organisation;
        }

        public RestrictionModel(AmphoraModel amphoraToRestrict, OrganisationModel targetOrganisation, RestrictionKind? kind = RestrictionKind.Deny)
        {
            TargetOrganisationId = targetOrganisation.Id;
            TargetOrganisation = targetOrganisation;
            SourceOrganisation = amphoraToRestrict.Organisation;
            SourceOrganisationId = amphoraToRestrict.OrganisationId;
            SourceAmphora = amphoraToRestrict;
            SourceAmphoraId = amphoraToRestrict.Id;
            Kind = kind;
            Scope = RestrictionScope.Amphora;
        }

        /// <summary>
        /// Gets or sets the kind of restriction, Allow or Deny
        /// </summary>
        public RestrictionKind? Kind { get; set; }

        /// <summary>
        /// Gets or sets the scope of restriction, Amphora or Organisation
        /// </summary>
        public RestrictionScope? Scope { get; set; }

        // Navigation

        /// <summary>
        /// Gets or sets the organisation that is restricted.
        /// </summary>
        public string? SourceOrganisationId { get; set; }

        /// <summary>
        /// Gets or sets the organisation that owns the restriction.
        /// </summary>
        public virtual OrganisationModel? SourceOrganisation { get; set; }
        /// <summary>
        /// Gets or sets the Amphora that is restricted.
        /// </summary>
        public string? SourceAmphoraId { get; set; }

        /// <summary>
        /// Gets or sets the Amphora that is being restricted.
        /// </summary>
        public virtual AmphoraModel? SourceAmphora { get; set; }

        /// <summary>
        /// Gets or sets the Organisation Id that is being restricted.
        /// </summary>
        public string? TargetOrganisationId { get; set; }

        /// <summary>
        /// Gets or sets the Organisation that is restricted.
        /// </summary>
        public virtual OrganisationModel? TargetOrganisation { get; set; }
    }
}