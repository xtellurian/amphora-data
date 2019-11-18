using System.Linq;

namespace Amphora.Common.Models.Organisations
{
    public class TermsAndConditionsAcceptanceModel
    {
        protected TermsAndConditionsAcceptanceModel(string termsAndConditionsId, string acceptedByOrganisationId, string termsAndConditionsOrganisationId )
        {
            TermsAndConditionsId = termsAndConditionsId;
            AcceptedByOrganisationId = acceptedByOrganisationId;
            TermsAndConditionsOrganisationId = termsAndConditionsOrganisationId;
        }
        public TermsAndConditionsAcceptanceModel(OrganisationModel acceptedByOrg, TermsAndConditionsModel termsToAccept)
        {
            this.AcceptedByOrganisationId = acceptedByOrg.Id;
            this.AcceptedByOrganisation = acceptedByOrg;

            this.TermsAndConditionsId = termsToAccept.Id;
            this.HasAccepted = true;

            this.TermsAndConditionsOrganisation = termsToAccept.Organisation;
            this.TermsAndConditionsOrganisationId = termsToAccept.OrganisationId;

        }
        public string TermsAndConditionsId { get; set; }
        public bool? HasAccepted { get; set; }
        // Navigation
        public string AcceptedByOrganisationId { get; set; } // the org that owns this 
        public virtual OrganisationModel AcceptedByOrganisation { get; set; } = null!;
        public string TermsAndConditionsOrganisationId { get; set; } // the other org
        public virtual OrganisationModel TermsAndConditionsOrganisation { get; set; } = null!;
    }
}