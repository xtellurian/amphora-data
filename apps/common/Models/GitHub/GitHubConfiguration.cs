namespace Amphora.Common.Models.GitHub
{
    public class GitHubConfiguration
    {
        public GitHubConfiguration()
        {
        }

        public GitHubConfiguration(string? productHeaderValue, string? token, string? defaultUser = null, string? defaultRepo = null)
        {
            ProductHeaderValue = productHeaderValue;
            DefaultUser = defaultUser;
            DefaultRepo = defaultRepo;
            Token = token;
        }

        public string? ProductHeaderValue { get; set; }
        public string? DefaultUser { get; set; }
        public string? DefaultRepo { get; set; }
        public string? Token { get; set; }
        public bool SuppressRateLimitExceptions { get; set; }
    }
}