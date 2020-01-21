namespace Amphora.Common.Contracts
{
    public interface IEmailRecipient
    {
        string Email { get; }
        string? FullName { get; }
    }
}