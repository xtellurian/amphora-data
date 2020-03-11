namespace Amphora.SharedUI.Models
{
    public class FooterModel
    {
        public FooterModel()
        { }
        public FooterModel(string stack, string version, string gitHash, bool enableValues = true, bool enableChangelog = true)
        {
            Stack = stack;
            Version = version;
            GitHash = gitHash;
            EnableValues = enableValues;
            EnableChangelog = enableChangelog;
        }

        public string Stack { get; }
        public string Version { get; }
        public string GitHash { get; }
        public bool EnableValues { get; }
        public bool EnableChangelog { get; }
    }
}