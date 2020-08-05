using System;
using System.Collections.Generic;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Users;
using Newtonsoft.Json;

namespace Amphora.Common.Models.Emails
{
    public class RequestToJoinEmail : EmailBase, IEmail
    {
        public static string TemplateName => "RequestToJoin";
        public static Dictionary<string, string?> TemplateData(ApplicationUserDataModel requester, OrganisationModel org)
        {
            return new Dictionary<string, string?>
            {
                { "{{Organisation_Name}}", org.Name },
                { "{{Invitee_Email}}", requester.ContactInformation?.Email },
                { "{{Invitee_Name}}", requester.UserName },
                { "{{Base_Url}}", BaseUrl },
            };
        }

        public RequestToJoinEmail(OrganisationModel org, string htmlContent)
        {
            // send this to the org owner and/or admins
            foreach (var member in org.Memberships)
            {
                if (member.User?.ContactInformation?.Email != null && member.Role == Roles.Administrator)
                {
                    Recipients.Add(new EmailRecipient(member.User.ContactInformation.Email,
                        member.User?.ContactInformation?.FullName ?? "Amphora User"));
                }
            }

            HtmlContent = htmlContent;
        }

        public override string HtmlContent { get; set; }
    }
}