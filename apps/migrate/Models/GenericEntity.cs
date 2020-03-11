using Newtonsoft.Json;

namespace Amphora.Migrate.Models
{
    public class GenericEntity
    {
        [JsonProperty("id")]
        public string? Id { get; set; }
    }
}