namespace Amphora.Common.Models.Organisations
{
    public class Membership 
    {
        public Membership() {}
        public Membership(string userId, string userName, Roles role)
        {
            UserModelId = userId;
            UserName = userName;
            Role = role;
        }
        public string UserModelId {get; set;}
        public string UserName {get; set;}
        public Roles Role {get; set; }
    }
}