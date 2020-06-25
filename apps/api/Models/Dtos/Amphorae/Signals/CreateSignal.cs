using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Amphora.Api.Models.Dtos.Amphorae.Signals
{
    public class CreateSignal
    {
        public CreateSignal()
        {
        }

        public CreateSignal(string property, string valueType, IDictionary<string, string> attributes)
        {
            Property = property;
            ValueType = valueType;
            Attributes = attributes;
        }

        [RegularExpression(@"^[a-z][a-zA-Z_]{2,20}$", ErrorMessage = "lowercase alpha, 3-20 chars")] // 20 lowercase alpha characters
        public string Property { get; set; }
        public string ValueType { get; set; }
        public IDictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
    }
}