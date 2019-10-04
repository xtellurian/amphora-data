using System.ComponentModel.DataAnnotations;
using Amphora.Common.Models.Signals;

namespace Amphora.Api.Models.Dtos.Amphorae
{
    public class SignalValueDto
    {
        public SignalValueDto()
        {
        }

        public SignalValueDto(string keyName, string valueType)
        {
            KeyName = keyName;
            ValueType = valueType;
        }
        public string KeyName { get; set; }
        public string ValueType { get; set; }
        public double NumericValue { get; set; }
        public string StringValue { get; set; }

        public bool IsNumeric => this.ValueType == SignalModel.Numeric;
        public bool IsString => this.ValueType == SignalModel.String;
    }
}