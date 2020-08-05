using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Amphora.Common.Contracts
{
    public interface IEmail
    {
        string Subject { get; set; }
        string HtmlContent { get; set; }
        [JsonIgnore]
        IList<IEmailRecipient> Recipients { get; }
    }
}
