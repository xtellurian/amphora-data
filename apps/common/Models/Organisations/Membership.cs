namespace Amphora.Common.Models.Organisations
{
    public class Membership 
    {
        public Membership() {}
        public Membership(string userId, string userName, Roles role)
        {
            UserId = userId;
            UserName = userName;
            Role = role;
        }
        public string UserId {get; set;}
        public string UserName {get; set;}
        public Roles Role {get; set; }
    }
}