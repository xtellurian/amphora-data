using Amphora.Common.Contracts;

namespace Amphora.Common.Models.UserData
{
    public class ApplicationUserReference : IApplicationUserReference
    {
        public string Id { get; set; }
        public string OrganisationId { get; set; }
        public string UserName { get; set; }
    }
}