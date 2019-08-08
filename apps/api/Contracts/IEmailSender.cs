using System.Threading.Tasks;

namespace Amphora.Api.Contracts
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}