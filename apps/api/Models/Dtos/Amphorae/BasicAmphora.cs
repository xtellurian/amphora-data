using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Amphora.Common.Models.Amphorae;

namespace Amphora.Api.Models.Dtos.Amphorae
{
    /// <summary>
    /// The basic metadata of an Amphora.
    /// </summary>
    public class BasicAmphora : Entity
    {
        [Required]
        [StringLength(120)]
        public string Name { get; set; }
        [Required]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "2 decimal places")]
        [DataType(DataType.Currency)]
        public double Price { get; set; }

        // number of labels is enforced by this REGEX
        [Display(Name = "Labels")]
        [RegularExpression(@"^(([-\w ]{0,12})[, ]?){1,11}$", ErrorMessage = "Comma Separated Labels, Max 10 Labels")]
        public string Labels { get; set; } = "";
        public List<Label> GetLabels()
        {
            this.Labels ??= ""; // set to empty string if null
            var labels = this.Labels.Trim().Split(',').ToList();
            return new List<Label>(labels
                .Where(name => !string.IsNullOrEmpty(name))
                .Select(name => new Label(name)));
        }
    }
}