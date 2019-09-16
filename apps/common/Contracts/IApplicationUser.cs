using System;

namespace Amphora.Common.Contracts
{
    public interface IApplicationUser
    {
        string Id { get; set; }
        string OrganisationId { get; set; }
        string UserName { get; set; }
        string Email { get; set; }
        string About { get; set; }
        string FullName { get; set; }
        Uri GetProfilePictureUri();
    }
}
