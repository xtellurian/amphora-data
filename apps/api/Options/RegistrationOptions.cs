namespace Amphora.Api.Options
{
    public class RegistrationOptions
    {
        public string Token { get; set; }
        public User RootUser {get; set; }
    }

    public class User
    {
        public string Email {get; set; }
        public string Token {get; set; }
    }
}