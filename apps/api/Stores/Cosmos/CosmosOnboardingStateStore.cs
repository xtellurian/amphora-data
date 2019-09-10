using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Options;
using Amphora.Common.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Amphora.Api.Stores.Cosmos
{
    public class CosmosOnboardingStateStore : CosmosStoreBase, IEntityStore<OnboardingState>
    {
        public CosmosOnboardingStateStore(IOptionsMonitor<CosmosOptions> options, ILogger<CosmosOnboardingStateStore> logger)
            : base(options, logger)
        {

        }
        public async Task<OnboardingState> CreateAsync(OnboardingState entity)
        {
            return await base.CreateAsync<OnboardingState>(entity);
        }

        public async Task DeleteAsync(OnboardingState entity)
        {
            await base.DeleteAsync(entity);
        }

        public Task<System.Collections.Generic.IList<OnboardingState>> ListAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<System.Collections.Generic.IList<OnboardingState>> ListAsync(string orgId)
        {
            throw new System.NotImplementedException();
        }

        public Task<System.Collections.Generic.IEnumerable<OnboardingState>> QueryAsync(System.Func<OnboardingState, bool> where)
        {
            throw new System.NotImplementedException();
        }

        public async Task<OnboardingState> ReadAsync(string id)
        {
            return await base.ReadAsync<OnboardingState>(id);
        }

        public async Task<OnboardingState> ReadAsync(string id, string orgId)
        {
            if(orgId == null) return await this.ReadAsync(id);
            return await base.ReadAsync<OnboardingState>(id, orgId);
        }

        public Task<System.Collections.Generic.IList<OnboardingState>> StartsWithQueryAsync(string propertyName, string givenValue)
        {
            throw new System.NotImplementedException();
        }

        public async Task<OnboardingState> UpdateAsync(OnboardingState entity)
        {
            return await base.UpdateAsync<OnboardingState>(entity);
        }
    }
}