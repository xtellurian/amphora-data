using System;
using System.Linq;
using System.Security.Claims;
using Amphora.Common.Models.Users;

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
            var email = principal.Claims.FirstOrDefault(_ => _.Type == "email")?.Value;
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
            return principal.Claims.FirstOrDefault(_ => _.Type == "name")?.Value;
        }

        public static string? GetFullName(this ClaimsPrincipal principal)
        {
            return principal.Claims.FirstOrDefault(_ => _.Type == "full_name")?.Value;
        }

        public static string? GetEmail(this ClaimsPrincipal principal, bool normalise = true)
        {
            var email = principal.Claims.FirstOrDefault(_ => _.Type == "email")?.Value;
            return normalise ? email?.ToUpper() ?? "" : email ?? "";
        }

        public static bool IsEmailConfirmed(this ClaimsPrincipal principal)
        {
            return principal.Claims.FirstOrDefault(_ => _.Type == "email_confirmed")?.Value == true.ToString();
        }

        public static bool IsGlobalAdmin(this ClaimsPrincipal principal)
        {
            return principal.Claims.FirstOrDefault(_ => _.Type == "global_admin")?.Value == true.ToString();
        }
    }
}