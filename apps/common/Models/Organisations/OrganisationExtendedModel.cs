using System.Collections.Generic;

namespace Amphora.Common.Models.Organisations
{
    public class OrganisationExtendedModel : OrganisationModel
    {
        public List<Invitation> Invitations { get; set; }
        public string About { get; set; }
        public string WebsiteUrl { get; set; }
        public string Address { get; set; }
        public void AddInvitation(string email)
        {
            if(this.Invitations == null) this.Invitations = new List<Invitation>();
            this.Invitations.Add(new Invitation(email));
        }
    }
}