namespace Amphora.Common.Contracts
{
    // an AmphoraEntity is a persistant object with a globally unique Id
    public interface IEntity
    {
        string Id { get; set; }

    }
}