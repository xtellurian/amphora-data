using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Amphora.Api.Models.Dtos.Amphorae
{
    public class SignalValuesDto
    {
        [Required]
        public List<PropertyValuePair> SignalValues { get; set; }
    }
}