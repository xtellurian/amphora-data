using System.Collections.Generic;
using System.Text.Json.Serialization;
using Amphora.Common.Contracts;

namespace Amphora.Common.Contracts
{
    public interface IEmail
    {
        string SendGridTemplateId { get; }
        [JsonIgnore]
        IList<IEmailRecipient> Recipients { get; }
    }
}
