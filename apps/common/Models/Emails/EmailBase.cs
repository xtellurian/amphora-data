using System.Collections.Generic;
using Amphora.Common.Contracts;
using Newtonsoft.Json;

namespace Amphora.Common.Models.Emails
{
    public abstract class EmailBase : IEmail
    {
        [JsonIgnore]
        public IList<IEmailRecipient> Recipients { get; private set; } = new List<IEmailRecipient>();

        public abstract string SendGridTemplateId { get; }

        [JsonProperty("baseUrl")]
        public string BaseUrl { get; set; } = AmphoraHost.MainHost;
    }
}