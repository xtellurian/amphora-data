using System;
using Amphora.Api.AspNet;
using Amphora.Common.Models.Amphorae;
using Newtonsoft.Json;

namespace Amphora.Api.Models.Dtos.Amphorae
{
    public class SignalValue
    {
        public SignalValue()
        {
        }

        public SignalValue(string property, string valueType) : this()
        {
            Property = property;
            ValueType = valueType;
        }

        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        public string Property { get; set; }

        /// <summary>
        /// Gets or sets one of Numeric, String, or DateTime.
        /// </summary>
        [StringRange(AllowableValues = new[] { SignalV2.Numeric, SignalV2.String, SignalV2.DateTime },
            ErrorMessage = "ValueType must be Numeric, String, or DateTime")]
        public string ValueType { get; set; }
        [JsonIgnore]
        public double NumericValue { get; set; }
        [JsonIgnore]
        public string StringValue { get; set; }
        [JsonIgnore]
        public DateTime? DateTimeValue { get; set; }
        private string GetValue()
        {
            if (IsNumeric) { return NumericValue.ToString(); }
            else if (IsDateTime) { return DateTimeValue?.ToString(); }
            else if (IsString) { return StringValue?.ToString(); }
            else { return null; }
        }

        private void SetValue(string value)
        {
            if (IsNumeric) { NumericValue = double.Parse(value); }
            if (IsDateTime) { DateTimeValue = DateTime.Parse(value); }
            if (IsString) { StringValue = value; }
        }

        /// <summary>
        /// Gets or sets the value (as a string, eg '7').
        /// </summary>
        public string Value { get => GetValue(); set => SetValue(value); }

        [JsonIgnore]
        public bool IsNumeric => this.ValueType == SignalV2.Numeric;
        [JsonIgnore]
        public bool IsString => this.ValueType == SignalV2.String;
        [JsonIgnore]
        public bool IsDateTime => this.ValueType == SignalV2.DateTime;
    }
}