using System.ComponentModel.DataAnnotations;

namespace Amphora.Api.Models.Dtos.Platform
{
    public class HandleInvitation
    {
        /// <summary>
        /// Gets or sets the Id of the organisation that issued the invitation.
        /// </summary>
        [Required]
        public string TargetOrganisationId { get; set; }

        /// <summary>
        /// Gets or sets whether the Invitation will be accepted. True == Accept. False == Reject.
        /// </summary>
        [Required]
        public bool? AcceptOrReject { get; set; }
    }
}