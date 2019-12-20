namespace Amphora.Api.Models.Versions
{
    public class VersionFile
    {
        public VersionFile(string versionName, string mdFilePath)
        {
            VersionName = versionName;
            MdFilePath = mdFilePath;
        }

        public string VersionName { get; set; }
        public string MdFilePath { get; set; }
    }
}