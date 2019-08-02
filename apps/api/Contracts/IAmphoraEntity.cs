namespace api.Contracts
{
    // an AmphoraEntity is a persistant object with a globally unique Id
    public interface IAmphoraEntity
    {
        string Id { get; set; }

    }
}