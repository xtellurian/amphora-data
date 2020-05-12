using System;
using Amphora.Common.Models.Permissions;

namespace Amphora.Common.Models.Organisations
{
    [Flags]
    public enum Roles
    {
        User = 0,
        Administrator = 1
    }

    public static class RolesExtensions
    {
        public static AccessLevels ToDefaultAccessLevel(this Roles role)
        {
            AccessLevels level = AccessLevels.None;
            switch (role)
            {
                case Roles.Administrator:
                    level = AccessLevels.Administer;
                    break;
                case Roles.User:
                    level = AccessLevels.CreateEntities;
                    break;
            }

            return level;
        }
    }
}