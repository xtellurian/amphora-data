using System;
using System.Linq;
using System.Security.Claims;

namespace Amphora.Common.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string? GetUserId(this ClaimsPrincipal principal)
        {
            var userId = principal.Claims.FirstOrDefault(_ => _.Type == "sub")?.Value;
            return userId;
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
            var userName = principal.Claims.FirstOrDefault(_ => _.Type == "name")?.Value;
            return userName;
        }
    }
}