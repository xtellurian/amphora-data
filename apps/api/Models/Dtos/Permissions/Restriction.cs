using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Amphora.Common.Models.Permissions;

namespace Amphora.Api.Models.Dtos.Permissions
{
    public class Restriction
    {
        public Restriction() { }
        public Restriction(string targetOrganisationId, RestrictionKind? kind = RestrictionKind.Deny)
        {
            TargetOrganisationId = targetOrganisationId;
            Kind = kind;
        }

        public string Id { get; set; }

        [Description("The kind of Restriction (Allow [default] or Deny)")]
        [Display(Name = "Restriction Scope")]
        public RestrictionKind? Kind { get; set; }

        [Description("Target Organisation's Id")]
        [Display(Name = "Target's Organisation Id")]
        [Required]
        public string TargetOrganisationId { get; set; }
    }
}