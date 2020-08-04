using System;

namespace Amphora.Api.Models.Dtos.Accounts
{
    public class CreateInvoice
    {
        public CreateInvoice(DateTimeOffset month, string organisationId, bool? preview, bool? regenerate)
        {
            Month = month;
            OrganisationId = organisationId;
            Preview = preview;
            Regenerate = regenerate;
        }

        public DateTimeOffset Month { get; set; }
        public string OrganisationId { get; set; }
        public bool? Preview { get; set; }
        public bool? Regenerate { get; set; }
    }
}