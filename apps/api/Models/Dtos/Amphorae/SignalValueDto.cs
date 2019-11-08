using System;
using Amphora.Api.AspNet;
using Amphora.Common.Models.Signals;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSwag.Annotations;

namespace Amphora.Api.Models.Dtos.Amphorae
{
    public class SignalValueDto
    {
        public SignalValueDto()
        {
        }

        public SignalValueDto(string property, string valueType) : this()
        {
            Property = property;
            ValueType = valueType;
        }
        /// <summary>
        /// The name of the property
        /// </summary>
        public string Property { get; set; }
        /// <summary>
        /// One of Numeric, String, or DateTime
        /// </summary>
        [StringRange(AllowableValues = new[] { SignalModel.Numeric, SignalModel.String, SignalModel.DateTime },
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
            if (IsNumeric) return NumericValue.ToString();
            else if (IsDateTime) return DateTimeValue?.ToString();
            else if (IsString) return StringValue?.ToString();
            else return null;
        }
        private void SetValue(string value)
        {
            if (IsNumeric) NumericValue = double.Parse(value);
            if (IsDateTime) DateTimeValue = DateTime.Parse(value);
            if (IsString) StringValue = value;
        }
        /// <summary>
        /// The value (as a string, eg '7')
        /// </summary>
        public string Value { get => GetValue(); set => SetValue(value); }

        [JsonIgnore]
        public bool IsNumeric => this.ValueType == SignalModel.Numeric;
        [JsonIgnore]
        public bool IsString => this.ValueType == SignalModel.String;
        [JsonIgnore]
        public bool IsDateTime => this.ValueType == SignalModel.DateTime;
    }
}