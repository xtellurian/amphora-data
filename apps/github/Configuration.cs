namespace Amphora.GitHub
{
    public class Configuration
    {
        public Configuration()
        {
        }

        public Configuration(string? productHeaderValue, string? token, string? defaultUser = null, string? defaultRepo = null)
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
    }
}