using System.Collections.Generic;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Platform;

namespace Amphora.Common.Models.Emails
{
    public class InvitationEmail : EmailBase, IEmail
    {
        public static string TemplateName => "Invitation";
        public static Dictionary<string, string> GetTemplateData(InvitationModel recipient, string? baseUrl = null)
        {
            return new Dictionary<string, string>
            {
                { "{{organisation_name}}", recipient.TargetOrganisation?.Name ?? recipient.TargetOrganisationId! },
                { "{{email}}", recipient?.TargetEmail ?? throw new System.ArgumentNullException(nameof(recipient) + " or recipient.TargetEmail") },
                { "{{base_url}}", baseUrl ?? BaseUrl },
            };
        }

        public InvitationEmail(InvitationModel recipient, string htmlContent) : base("You've been invited to Amphora Data")
        {
            if (recipient is null || recipient.TargetEmail is null)
            {
                throw new System.ArgumentNullException(nameof(recipient) + " or recipient.TargetEmail");
            }

            this.Recipients.Add(new EmailRecipient(recipient.TargetEmail, ""));

            HtmlContent = htmlContent;
        }

        public override string HtmlContent { get; set; }
    }
}
