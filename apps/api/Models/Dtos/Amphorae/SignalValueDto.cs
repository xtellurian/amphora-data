using System;
using Amphora.Common.Models.Signals;

namespace Amphora.Api.Models.Dtos.Amphorae
{
    public class SignalValueDto
    {
        public SignalValueDto()
        {
        }

        public SignalValueDto(string property, string valueType): this()
        {
            Property = property;
            ValueType = valueType;
        }
        public string Property { get; set; }
        public string ValueType { get; set; }
        public double NumericValue { get; set; }
        public string StringValue { get; set; }
        public DateTime? DateTimeValue { get; set; }

        public bool IsNumeric => this.ValueType == SignalModel.Numeric;
        public bool IsString => this.ValueType == SignalModel.String;
        public bool IsDateTime => this.ValueType == SignalModel.DateTime;
    }
}