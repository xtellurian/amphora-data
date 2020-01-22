using Amphora.Common.Contracts;

namespace Amphora.Api.Models.Emails
{
    public class EmailRecipient : IEmailRecipient
    {
        public EmailRecipient(string email, string fullName)
        {
            Email = email;
            FullName = fullName;
        }

        public string Email { get; private set; }

        public string FullName { get; private set; }
    }
}