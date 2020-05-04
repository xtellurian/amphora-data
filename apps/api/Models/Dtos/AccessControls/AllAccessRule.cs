namespace Amphora.Api.Models.Dtos.AccessControls
{
    public class AllAccessRule : AccessRuleDtoBase
    {
        public static AllAccessRule Deny(int priority = 100)
        {
            return AccessRuleDtoBase.Deny<AllAccessRule>(priority);
        }

        public static AllAccessRule Allow(int priority = 100)
        {
            return AccessRuleDtoBase.Allow<AllAccessRule>(priority);
        }
    }
}