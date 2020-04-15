using System.Threading.Tasks;

namespace Amphora.Common.Contracts
{
    public interface IAmphoraFileReference
    {
        string Name { get; }
        System.DateTimeOffset? LastModified { get; }
        Task LoadAttributesAsync();
    }
}