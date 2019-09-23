using System;
using Amphora.Api.Extensions;
using Amphora.Common.Contracts;

namespace Amphora.Api.Models.Users
{
    public class ApplicationUserDto : IApplicationUser
    {
        public string Email {get;set;}
        public string About  {get;set;}
        public string FullName  {get;set;}
        public string Id  {get;set;}
        public string OrganisationId  {get;set;}
        public string UserName  {get;set;}
        public Uri GetProfilePictureUri()
        {
            return new Uri($"https://www.gravatar.com/avatar/{GravatarExtensions.HashEmailForGravatar(this.Email)}");
        }
    }
}