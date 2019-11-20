using System.ComponentModel;
using Amphora.Common.Models.Permissions;

namespace Amphora.Api.Models.Dtos.Organisations
{
    public class Restriction
    {
        public RestrictionKind Kind { get; set; }
        [Description("Target Organisation's Id")]
        public string TargetOrganisationId { get; set; }
    }
}