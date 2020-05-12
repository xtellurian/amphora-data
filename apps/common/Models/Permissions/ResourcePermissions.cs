using Amphora.Common.Models.Permissions;

namespace Amphora.Common.Models
{
    public static class ResourcePermissions
    {
        public static AccessLevels Create => AccessLevels.CreateEntities;
        public static AccessLevels Read => AccessLevels.Read;
        public static AccessLevels Update => AccessLevels.Update;
        public static AccessLevels Delete => AccessLevels.Administer;

        // specific to amphorae contents
        public static AccessLevels ReadContents => AccessLevels.ReadContents;
        public static AccessLevels WriteContents => AccessLevels.WriteContents;
    }
}