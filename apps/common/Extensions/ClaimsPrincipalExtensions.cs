using System;
using System.Linq;
using System.Security.Claims;
using Amphora.Common.Security;

namespace Amphora.Common.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string? GetUserId(this ClaimsPrincipal principal)
        {
            if (principal.Identity.IsAuthenticated)
            {
                var userId = principal.Claims.FirstOrDefault(_ => _.Type == "sub")?.Value;
                return userId;
            }
            else
            {
                return null;
            }
        }

        public static Uri? GetProfilePictureUri(this ClaimsPrincipal principal)
        {
            var email = principal.Claims.FirstOrDefault(_ => _.Type == Claims.Email)?.Value;
            if (email != null)
            {
                return new Uri($"https://www.gravatar.com/avatar/{GravatarExtensions.HashEmailForGravatar(email)}");
            }
            else
            {
                return null;
            }
        }

        public static string? GetUserName(this ClaimsPrincipal principal)
        {
            return principal.Claims.FirstOrDefault(_ => _.Type == Claims.UserName)?.Value;
        }

        public static string? GetFullName(this ClaimsPrincipal principal)
        {
            return principal.Claims.FirstOrDefault(_ => _.Type == Claims.FullName)?.Value;
        }

        public static string? GetEmail(this ClaimsPrincipal principal, bool normalise = true)
        {
            var email = principal.Claims.FirstOrDefault(_ => _.Type == Claims.Email)?.Value;
            return normalise ? email?.ToLower() ?? "" : email ?? "";
        }

        public static string? GetAbout(this ClaimsPrincipal principal, bool normalise = true)
        {
            return principal.Claims.FirstOrDefault(_ => _.Type == Claims.About)?.Value;
        }

        public static bool IsEmailConfirmed(this ClaimsPrincipal principal)
        {
            return principal.Claims.FirstOrDefault(_ => _.Type == Claims.EmailConfirmed)?.Value == true.ToString();
        }

        public static bool IsGlobalAdmin(this ClaimsPrincipal principal)
        {
            return principal.Claims.FirstOrDefault(_ => _.Type == Claims.GlobalAdmin)?.Value == true.ToString();
        }
    }
}