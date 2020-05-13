using System.ComponentModel.DataAnnotations;

namespace Amphora.Api.Models.Dtos.Activities
{
    public class CreateActivity
    {
        public CreateActivity(string name)
        {
            Name = name;
        }

        [StringLength(120, MinimumLength = 3,
            ErrorMessage = "The {0} value cannot exceed {1} characters, and must be more than 3 characters.")]
        public string Name { get; set; }
    }
}