using System.ComponentModel.DataAnnotations;

namespace Amphora.Api.Models.Dtos.Platform
{
    public class AcceptInvitationDto
    {
        [Required]
        public string TargetOrganisationId { get; set; }
    }
}