using System.Collections.Generic;
using System.Text.Json.Serialization;
using Amphora.Common.Contracts;

namespace Amphora.Api.Models.Emails
{
    public abstract class EmailBase : IEmail
    {
        [JsonIgnore]
        public IList<IEmailRecipient> Recipients { get; private set; } = new List<IEmailRecipient>();

        public abstract string SendGridTemplateId { get; }
    }
}