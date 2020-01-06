using System;
using Amphora.Common.Contracts;
using Amphora.Common.Extensions;

namespace Amphora.Common.Models.Users
{
    public class UserDto : IUser
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string About { get; set; }
        public string FullName { get; set; }
        public string OrganisationId { get; set; }
        public string UserName { get; set; }
        public DateTimeOffset? LastModified { get; set; }

        public Uri GetProfilePictureUri()
        {
            return new Uri($"https://www.gravatar.com/avatar/{GravatarExtensions.HashEmailForGravatar(this.Email)}");
        }
    }
}