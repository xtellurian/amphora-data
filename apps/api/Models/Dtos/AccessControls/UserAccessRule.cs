using System.ComponentModel.DataAnnotations;

namespace Amphora.Api.Models.Dtos.AccessControls
{
    public class UserAccessRule : AccessRuleDtoBase
    {
        [Required]
        public string Username { get; set; }

        public static UserAccessRule Deny(string username, int priority = 100)
        {
            var rule = AccessRuleDtoBase.Deny<UserAccessRule>();
            rule.Username = username;
            return rule;
        }

        public static UserAccessRule Allow(string username, int priority = 100)
        {
            var rule = AccessRuleDtoBase.Allow<UserAccessRule>();
            rule.Username = username;
            return rule;
        }
    }
}