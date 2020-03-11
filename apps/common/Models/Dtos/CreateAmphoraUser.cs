using System.ComponentModel.DataAnnotations;
using Amphora.Common.Contracts;

namespace Amphora.Common.Models.Dtos
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