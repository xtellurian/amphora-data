using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Amphora.Common.DataAnnotations
{
    public class OneOfAttribute : ValidationAttribute
    {
        private readonly string[] allowedValues;

        public OneOfAttribute(params string[] allowedValues)
        {
            this.allowedValues = allowedValues ?? new string[0];
        }

        /// <summary>
        /// Determines whether the specified value of the object is valid.
        /// </summary>
        /// <returns>
        /// A ValidationResult specifying whether the value is valid.
        /// </returns>
        /// <param name="value">The value of the specified validation object on which the <see cref="T:System.ComponentModel.DataAnnotations.ValidationAttribute"/> is declared.
        /// </param>
        /// <param name="validationContext">The context object, the <see cref="T:System.ComponentModel.DataAnnotations.ValidationAttribute"/>.
        /// </param>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Value cannot be null");
            }
            else if (value.GetType() != typeof(string))
            {
                return new ValidationResult("Value must be a string");
            }
            else
            {
                var s = (string)value;
                if (allowedValues.Any(_ => string.Equals(s, _)))
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult($"Value must be one of: {string.Join(',', allowedValues)}");
                }
            }
        }
    }
}