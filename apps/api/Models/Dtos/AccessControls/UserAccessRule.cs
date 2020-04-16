using System.ComponentModel.DataAnnotations;

namespace Amphora.Api.Models.Dtos.AccessControls
{
    public class UserAccessRule : AccessRuleDtoBase
    {
        [Required]
        public string Username { get; set; }
    }
}