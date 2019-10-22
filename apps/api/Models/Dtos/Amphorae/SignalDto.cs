using System.ComponentModel.DataAnnotations;

namespace Amphora.Api.Models.Dtos.Amphorae
{
    public class SignalDto
    {
        public string Id { get; set; }
        [RegularExpression(@"^[a-zA-Z]{1,20}$", ErrorMessage = "Characters are not allowed.")] // 20 alpha characters
        public string Property { get; set; }
        public string ValueType { get; set; }
    }
}