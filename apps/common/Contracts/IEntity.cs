namespace Amphora.Common.Contracts
{
    // an IEntity is a persistent object with a globally unique Id
    public interface IEntity
    {
        string Id { get; set; }

    }
}