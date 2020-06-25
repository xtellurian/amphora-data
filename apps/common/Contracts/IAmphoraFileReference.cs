using System.Collections.Generic;

namespace Amphora.Common.Contracts
{
    public interface IAmphoraFileReference
    {
        string Name { get; }
        System.DateTimeOffset? LastModified { get; }
        IDictionary<string, string> Metadata { get; }
    }
}