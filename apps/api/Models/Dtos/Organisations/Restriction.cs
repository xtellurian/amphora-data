using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Amphora.Common.Models.Permissions;

namespace Amphora.Api.Models.Dtos.Organisations
{
    public class Restriction
    {
        public Restriction() { }
        public Restriction(string targetOrganisationId, RestrictionKind? kind = RestrictionKind.Deny)
        {
            TargetOrganisationId = targetOrganisationId;
            Kind = kind;
        }

        [Description("The kind of Restriction (Allow [default] or Deny)")]
        [Display(Name = "Restriction Scope")]
        public RestrictionKind? Kind { get; set; }

        [Description("Target Organisation's Id")]
        [Display(Name = "Target's Organisation Id")]
        public string TargetOrganisationId { get; set; }
    }
}