using System.ComponentModel.DataAnnotations;

namespace Amphora.Api.Models.Dtos.Platform
{
    public class Invitation
    {
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Target Email Address")]
        [Required]
        public string TargetEmail { get; set; }
        public string TargetOrganisationId { get; set; }
        public string State { get; set; }
    }
}