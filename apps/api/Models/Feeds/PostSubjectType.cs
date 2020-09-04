using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace Amphora.Api.Models.Feeds
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PostSubjectType
    {
        Amphora
    }
}