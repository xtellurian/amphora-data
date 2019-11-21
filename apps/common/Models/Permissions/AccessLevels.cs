namespace Amphora.Common.Models.Permissions
{
    public enum AccessLevels
    {
        None = 0,
        Read = 16,
        Purchase = 24,
        ReadContents = 32,
        WriteContents = 64,
        Update = 128,
        Administer = 256
    }

    public static class AccessLevelsExtensions
    {
        public static string FriendlyName(this AccessLevels level)
        {
            return System.Enum.GetName(typeof(AccessLevels), level);
        }
    }
}
