using System.Collections.Generic;

namespace Amphora.Common.Models.Organisations
{
    public class OrganisationExtendedModel : OrganisationModel
    {
        public List<Invitation> Invitations { get; set; }
    }
}