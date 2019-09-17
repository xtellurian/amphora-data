using System;

namespace Amphora.Common.Contracts
{
    public interface IApplicationUserReference
    {
        string Id { get; set; }
        string OrganisationId { get; set; }
        string UserName { get; set; }
    }
    public interface IApplicationUser : IApplicationUserReference
    {
        string Email { get; set; }
        string About { get; set; }
        string FullName { get; set; }
        Uri GetProfilePictureUri();
    }
}
