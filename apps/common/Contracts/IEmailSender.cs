using System.Threading.Tasks;

namespace Amphora.Common.Contracts
{
    public interface IEmailSender
    {
        IEmailGenerator Generator { get; }
        Task SendEmailAsync(string email, string subject, string message);
        Task<bool> SendEmailAsync(IEmail email);
    }
}