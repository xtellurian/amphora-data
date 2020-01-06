using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Amphora.Common.Models.Amphorae;

namespace Amphora.Api.Models.Dtos.Amphorae
{
    public class AmphoraDto : EntityDto
    {

        [Required]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public double Price { get; set; }
        [Display(Name = "Labels")]
        [RegularExpression(@"^(([-\w ]{0,12})[, ]?){1,6}$", ErrorMessage = "Comma Separated Labels, Max 5 Labels")]
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