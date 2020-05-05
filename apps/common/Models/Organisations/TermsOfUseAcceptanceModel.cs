using Amphora.Common.Models.Amphorae;

namespace Amphora.Common.Models.Organisations
{
    public class TermsOfUseAcceptanceModel
    {
        protected TermsOfUseAcceptanceModel(string termsOfUseId, string acceptedByOrganisationId, string termsOfUseOrganisationId)
        {
            TermsOfUseId = termsOfUseId;
            AcceptedByOrganisationId = acceptedByOrganisationId;
            TermsOfUseOrganisationId = termsOfUseOrganisationId;
        }

        public TermsOfUseAcceptanceModel(OrganisationModel acceptedByOrg, TermsOfUseModel termsToAccept)
        {
            this.AcceptedByOrganisationId = acceptedByOrg.Id;
            this.AcceptedByOrganisation = acceptedByOrg;

            this.TermsOfUseId = termsToAccept.Id;
            this.HasAccepted = true;

            this.TermsOfUseOrganisation = termsToAccept.Organisation;
            this.TermsOfUseOrganisationId = termsToAccept.OrganisationId;
        }

        public string Id { get; set; } = null!;
        public string TermsOfUseId { get; set; }
        public bool? HasAccepted { get; set; }
        // Navigation
        public string AcceptedByOrganisationId { get; set; } // the org that owns this
        public virtual OrganisationModel AcceptedByOrganisation { get; set; } = null!;
        public string? TermsOfUseOrganisationId { get; set; } // the other org
        public virtual OrganisationModel? TermsOfUseOrganisation { get; set; } = null!;
    }
}