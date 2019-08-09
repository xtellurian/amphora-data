using Microsoft.AspNetCore.Identity;

namespace Amphora.Api.Models.Development
{
    public class DevSignInResult : SignInResult
    {
        public static bool Disabled { get; set; }
        public DevSignInResult(bool success)
        {
            if(Disabled) throw new System.AccessViolationException("Sign in Result disabled.");
            this.Succeeded = success;
        }
    }
}