using System.ComponentModel.DataAnnotations;

namespace Amphora.Api.Models.Dtos.Accounts.Memberships
{
    public class Invitation : IDto
    {
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Target Email Address")]
        [Required]
        public string TargetEmail { get; set; }
        public string TargetOrganisationId { get; set; }
        public string State { get; set; }
    }
}