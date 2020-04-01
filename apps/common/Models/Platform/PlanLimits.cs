namespace Amphora.Common.Models.Platform
{
    public class PlanLimits
    {
        public PlanLimits(long maxStorageInBytes, int maxUsers)
        {
            MaxStorageInBytes = maxStorageInBytes;
            MaxUsers = maxUsers;
        }

        public long MaxStorageInBytes { get; }
        public int MaxUsers { get; }
    }
}