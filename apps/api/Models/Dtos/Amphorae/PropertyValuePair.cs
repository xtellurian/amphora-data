using System.ComponentModel.DataAnnotations;

namespace Amphora.Api.Models.Dtos.Amphorae
{
    public class PropertyValuePair
    {
        public PropertyValuePair() { }
        public PropertyValuePair(string property, double value)
        {
            Property = property;
            NumericValue = value;
        }
        public PropertyValuePair(string property, string value)
        {
            Property = property;
            StringValue = value;
        }
        [Required]
        public string Property { get; set; }
        public double? NumericValue { get; set; }
        public string StringValue { get; set; }

        public bool IsString()
        {
            return !string.IsNullOrEmpty(StringValue);
        }

        public bool IsNumeric()
        {
            return NumericValue.HasValue;
        }
    }
}