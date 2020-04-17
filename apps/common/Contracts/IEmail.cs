using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Amphora.Common.Contracts
{
    public interface IEmail
    {
        string SendGridTemplateId { get; }
        [JsonIgnore]
        IList<IEmailRecipient> Recipients { get; }
    }
}
