namespace Amphora.Common.Models.Activities
{
    public class VersionInfo
    {
        public VersionInfo() { }

        public VersionInfo(int major, int minor, int patch)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
        }

        public int Major { get; set; }
        public int Minor { get; set; }
        public int Patch { get; set; }
    }
}