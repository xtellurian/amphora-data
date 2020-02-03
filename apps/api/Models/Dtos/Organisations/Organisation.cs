namespace Amphora.Api.Models.Dtos.Organisations
{
    public class Organisation : Entity
    {
        public string Name { get; set; }
        public string About { get; set; }
        public string WebsiteUrl { get; set; }
        public string Address { get; set; }
    }
}