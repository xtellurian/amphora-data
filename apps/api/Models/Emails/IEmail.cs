using System.ComponentModel.DataAnnotations;

namespace Amphora.Api.Models.Emails
{
    public interface IEmail
    {
        string SendGridTemplateId { get; }
        [DataType(DataType.EmailAddress)]
        string ToEmail { get; }
        string ToName { get; }
    }
}
