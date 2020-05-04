using System.ComponentModel.DataAnnotations;
using Amphora.Common.DataAnnotations;
using Amphora.Common.Models.Permissions.Rules;

namespace Amphora.Api.Models.Dtos.AccessControls
{
    public abstract class AccessRuleDtoBase
    {
        public static T Deny<T>(int priority = 100) where T : AccessRuleDtoBase, new() => new T
        {
            AllowOrDeny = "Deny",
            Priority = priority
        };

        public static T Allow<T>(int priority = 100) where T : AccessRuleDtoBase, new() => new T
        {
            AllowOrDeny = "Allow",
            Priority = priority
        };

        public string Id { get; set; }
        [Display(Name = "Allow or Deny")]
        [OneOf("Allow", "Deny")]
        public string AllowOrDeny { get; set; }
        public int Priority { get; set; } = 100;

        public Kind GetKind()
        {
            if (AllowOrDeny?.ToLower() == "allow")
            {
                return Kind.Allow;
            }
            else
            {
                return Kind.Deny;
            }
        }
    }
}