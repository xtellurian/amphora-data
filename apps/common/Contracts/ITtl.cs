namespace Amphora.Common.Contracts
{
    public interface ITtl: IEntity
    {
        int? ttl { get; set; } // don't expire
    }
}