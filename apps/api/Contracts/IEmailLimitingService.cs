namespace Amphora.Api.Contracts
{
    public interface IEmailLimitingService
    {
        bool CanSignup(string email);
    }
}