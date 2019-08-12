namespace Amphora.Common.Contracts
{
    public interface IOrgScoped : IEntity
    {
        string OrgId { get; set; }
    }
}