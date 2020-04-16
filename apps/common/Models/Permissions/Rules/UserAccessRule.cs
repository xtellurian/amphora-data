using Amphora.Common.Models.Users;

namespace Amphora.Common.Models.Permissions.Rules
{
    public class UserAccessRule : AccessRule
    {
        public UserAccessRule() : base()
        { }

        public UserAccessRule(Kind? kind, int? priority, ApplicationUserDataModel userData)
        : base(kind, priority)
        {
            UserDataId = userData.Id;
            UserData = userData;
        }

        public string? UserDataId { get; set; }
        public ApplicationUserDataModel? UserData { get; set; }

        public override string Name()
        {
            return $"{this.Kind} User {this.UserData?.UserName}";
        }
    }
}