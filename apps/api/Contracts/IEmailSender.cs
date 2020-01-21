using System.Threading.Tasks;
using Amphora.Api.Models.Emails;

namespace Amphora.Api.Contracts
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
        Task<bool> SendEmailAsync(IEmail email);
    }
}