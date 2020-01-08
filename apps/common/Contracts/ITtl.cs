namespace Amphora.Common.Contracts
{
    public interface ITtl : IEntity
    {
        #pragma warning disable SA1300 // Code should not contain trailing whitespace
        int? ttl { get; set; } // don't expire
        #pragma warning restore SA1028 // Code should not contain trailing whitespace
    }
}