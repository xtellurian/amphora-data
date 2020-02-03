using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Amphora.Api.Models.Dtos.Amphorae
{
    public class Signal
    {
        public Signal()
        {
        }

        public Signal(string id, string property, string valueType, IDictionary<string, string> meta)
        {
            Id = id;
            Property = property;
            ValueType = valueType;
            Meta = meta;
        }

        public string Id { get; set; }
        [RegularExpression(@"^[a-zA-Z_]{3,20}$", ErrorMessage = "lowercase alpha, 3-20 chars")] // 20 lowercase alpha characters
        public string Property { get; set; }
        public string ValueType { get; set; }
        public IDictionary<string, string> Meta { get; set; } = new Dictionary<string, string>();
    }
}