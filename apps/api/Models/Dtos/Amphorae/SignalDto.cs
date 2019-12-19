using System.ComponentModel.DataAnnotations;

namespace Amphora.Api.Models.Dtos.Amphorae
{
    public class SignalDto
    {
        public string Id { get; set; }
        [RegularExpression(@"^[a-zA-Z_]{3,20}$", ErrorMessage = "lowercase alpha, 3-20 chars")] // 20 lowercase alpha characters
        public string Property { get; set; }
        public string ValueType { get; set; }
    }
}