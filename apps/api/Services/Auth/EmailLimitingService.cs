using Amphora.Api.Contracts;

namespace Amphora.Api.Services.Auth
{
    public class EmailLimitingService: IEmailLimitingService
    {
        public bool CanSignup(string email)
        {
            if(email == null) return false;
            if(email.EndsWith("@amphoradata.com"))
            {
                return true;
            }
            else if(email.EndsWith("@acme.org"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}