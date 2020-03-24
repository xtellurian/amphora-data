using Amphora.Common.Models.Platform;

namespace Amphora.SharedUI.Models
{
    public class FooterModel
    {
        public FooterModel()
        { }
        public FooterModel(EnvironmentInfo envInfo, string version, string gitHash, bool enableValues = true, bool enableChangelog = true)
        {
            Stack = envInfo?.Stack;
            Location = envInfo?.Location;
            Version = version;
            GitHash = gitHash;
            EnableValues = enableValues;
            EnableChangelog = enableChangelog;
        }

        public string Stack { get; }
        public string Location { get; }
        public string Version { get; }
        public string GitHash { get; }
        public bool EnableValues { get; }
        public bool EnableChangelog { get; }
    }
}