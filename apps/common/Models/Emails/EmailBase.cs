using System.Collections.Generic;
using Amphora.Common.Contracts;
using Newtonsoft.Json;

namespace Amphora.Common.Models.Emails
{
    public abstract class EmailBase : IEmail
    {
        protected EmailBase(string? subject = null)
        {
            Subject = subject ?? "A message from Amphora Data";
        }

        [JsonIgnore]
        public IList<IEmailRecipient> Recipients { get; private set; } = new List<IEmailRecipient>();
        public static string BaseUrl => AmphoraHost.GetHost();
        public abstract string HtmlContent { get; set; }
        public virtual string Subject { get; set; }
        public virtual string Category => GetType().Name;
    }
}