using System;

namespace Amphora.Common.Models
{
    public class RoleAssignment
    {
        public RoleAssignment(){}

        public RoleAssignment(string userId, Roles role)
        {
            this.UserId = userId;
            this.Role = role;
        }
        public string UserId {get; set; }
        public Roles Role { get; set; }

        [Flags]
        public enum Roles
        {
            User = 0,
            Administrator = 1
        }
    }
}