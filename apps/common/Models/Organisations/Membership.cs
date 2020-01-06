using Amphora.Common.Models.Users;

namespace Amphora.Common.Models.Organisations
{
    public class Membership
    {
        public Membership(string userId)
        {
            UserId = userId;
        }

        public Membership(ApplicationUser user, Roles role)
        {
            User = user;
            UserId = user.Id;
            Role = role;
        }

        public Roles Role { get; set; }
        // Navigation
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; } = null!;
    }
}