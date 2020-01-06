using System;
using System.Collections.Generic;

namespace Amphora.Common.Models.Signals
{
    public class SignalModel : Entity
    {
        public const string Numeric = nameof(Numeric);
        public const string String = nameof(String);
        public const string DateTime = nameof(DateTime);
        public static List<string> Options => new List<string> { Numeric, String };

        public SignalModel(string property, string valueType)
        {
            Property = property ?? throw new System.ArgumentNullException(nameof(property));

            if (string.Equals(valueType, Numeric) || string.Equals(valueType, String) || string.Equals(valueType, DateTime))
            {
                ValueType = valueType;
            }
            else
            {
                throw new ArgumentException("ValueType must be Numeric, String, or DateTime");
            }

            CreatedDate = System.DateTime.UtcNow;
            LastModified = System.DateTime.UtcNow;

            this.Id = $"{Property}-{ValueType}";
        }

        public string Property { get; set; }
        public string ValueType { get; set; }

        public bool IsNumeric => this.ValueType == Numeric;
        public bool IsString => this.ValueType == String;
    }
}