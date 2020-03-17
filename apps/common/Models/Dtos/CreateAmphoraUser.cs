using System.ComponentModel.DataAnnotations;

namespace Amphora.Common.Models.Dtos.Users
{
    public class CreateAmphoraUser
    {
        [Required]
        public string? UserName { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        public string? Email { get; set; }
        public string? About { get; set; }
        public string? FullName { get; set; }
    }
}