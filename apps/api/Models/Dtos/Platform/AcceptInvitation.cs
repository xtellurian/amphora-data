using System.ComponentModel.DataAnnotations;

namespace Amphora.Api.Models.Dtos.Platform
{
    public class AcceptInvitation
    {
        [Required]
        public string TargetOrganisationId { get; set; }
    }
}