using System;
using common.Contracts;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Amphora.Common.Models
{
    public class AmphoraModel : IAmphoraEntity
    {
        public string Id { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public AmphoraClass Class { get; set; }
        public string SchemaId { get; set; }
        public bool Bounded { get; set; } = true; // a bounded amphora cannot be appended to
        public string Title { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
    }
}