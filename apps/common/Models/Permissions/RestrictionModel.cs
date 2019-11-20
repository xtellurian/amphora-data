using Amphora.Common.Models.Organisations;

namespace Amphora.Common.Models.Permissions
{
    /// <summary>
    /// Custom access restrictions to data in Amphora
    /// </summary>
    public class RestrictionModel
    {
        public RestrictionModel()
        {
            TargetOrganisationId = null!;
            TargetOrganisation = null!;
        }
        public RestrictionModel(string targetOrganisationId)
        {
            TargetOrganisationId = targetOrganisationId;
            TargetOrganisation = null!;
        }
        /// <summary>
        /// This restriction applies to all organisations.
        /// TargetOrganisationId is ignored.
        /// </summary>
        public bool? AllOrganisations { get; set; }
        /// <summary>
        /// The kind of restriction, Allow or Deny
        /// </summary>
        public RestrictionKind? Kind { get; set; }
        /// <summary>
        /// The organisation that is restricted.
        /// </summary>


        // Navigation
        public string? SourceOrganisationId { get; set; }
        /// <summary>
        /// The organisation that owns the restriction.
        /// </summary>
        public virtual OrganisationModel? SourceOrganisation { get; set; }

        public string? TargetOrganisationId { get; set; }
        /// <summary>
        /// The organisation that is restricted.
        /// </summary>
        public virtual OrganisationModel TargetOrganisation { get; set; }
    }

    public enum RestrictionKind
    {
        Deny,
        Allow
    }
}