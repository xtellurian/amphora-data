using Amphora.Common.Models.Organisations;

namespace Amphora.Common.Models.Permissions
{
    /// <summary>
    /// Custom access restrictions to data in Amphora
    /// </summary>
    public class Restriction
    {
        public const string MATCH_ALL = "*";
        public Restriction()
        {
            TargetOrganisationId = null!;
        }
        public Restriction(string targetOrganisationId)
        {
            TargetOrganisationId = targetOrganisationId;
        }
        /// <summary>
        /// The kind of restriction, Allow or Deny
        /// </summary>
        public RestrictionKind Kind { get; set; }
        /// <summary>
        /// The organisation that is restricted, Use * to match all.
        /// </summary>
        public string TargetOrganisationId { get; set; }
    }

    public enum RestrictionKind
    {
        Deny,
        Allow
    }
}