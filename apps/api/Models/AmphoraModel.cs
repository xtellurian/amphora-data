using System;
using api.Contracts;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace api.Models
{
    public class AmphoraModel : IAmphoraEntity
    {
        public string Id { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public AmphoraClass? Class { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
    }
}