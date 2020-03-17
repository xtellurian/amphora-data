namespace Amphora.Common.Contracts
{
    public interface IUser : IEntity
    {
        string? UserName { get; }
        string? OrganisationId { get; }
    }
}