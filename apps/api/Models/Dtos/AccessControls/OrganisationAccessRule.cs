using System.ComponentModel.DataAnnotations;

namespace Amphora.Api.Models.Dtos.AccessControls
{
    public class OrganisationAccessRule : AccessRuleDtoBase
    {
        public static OrganisationAccessRule Deny(string orgId, int priority = 100)
        {
            var rule = AccessRuleDtoBase.Deny<OrganisationAccessRule>();
            rule.OrganisationId = orgId;
            return rule;
        }

        public static OrganisationAccessRule Allow(string orgId, int priority = 100)
        {
            var rule = AccessRuleDtoBase.Allow<OrganisationAccessRule>();
            rule.OrganisationId = orgId;
            return rule;
        }

        [Required]
        public string OrganisationId { get; set; }
    }
}