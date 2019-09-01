namespace Amphora.Common.Models
{
    public class OrganisationMembership : Entity
    {
        public string OrganisationId { get; set; }
        public string Role { get; set; }
        public string UserId { get; set; }
    }
}