using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Common.Contracts;

namespace Amphora.Api.Contracts
{
    public interface IOrgScopedEntityStore<T> : IEntityStore<T> where T : IOrgScoped
    {
        Task<T> ReadAsync(string id, string orgId);
        Task<IList<T>> ListAsync(string orgId);
    }
}