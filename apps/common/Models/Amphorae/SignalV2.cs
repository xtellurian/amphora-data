using System;
using System.Collections.Generic;

namespace Amphora.Common.Models.Amphorae
{
    public class SignalV2
    {
        public const string Numeric = nameof(Numeric);
        public const string String = nameof(String);
        public const string DateTime = nameof(DateTime);
        public static List<string> Options => new List<string> { Numeric, String };

        public SignalV2(string property, string valueType)
        {
            Property = property ?? throw new System.ArgumentNullException(nameof(property));
            this.Id = $"{property}-{valueType}";
            if (string.Equals(valueType, Numeric) || string.Equals(valueType, String) || string.Equals(valueType, DateTime))
            {
                ValueType = valueType;
            }
            else
            {
                throw new ArgumentException("ValueType must be Numeric, String, or DateTime");
            }
        }

        public string? UUID { get; set; } // workaround the fact that entities owned by different parents can have same Id
        public string Id { get; set; }
        public string Property { get; set; }
        public string ValueType { get; set; }
        public AttributeStore Attributes { get; set; } = new AttributeStore();

        public bool IsNumeric => this.ValueType == Numeric;
        public bool IsString => this.ValueType == String;
    }
}