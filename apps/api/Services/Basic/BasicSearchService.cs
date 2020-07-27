using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Search;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.DataRequests;
using Amphora.Common.Models.Organisations;

namespace Amphora.Api.Services.Basic
{
    public class BasicSearchService : ISearchService
    {
        private readonly IAmphoraeService amphoraeService;
        private readonly IEntityStore<DataRequestModel> dataRequestStore;
        private readonly IEntityStore<OrganisationModel> organisationStore;

        public BasicSearchService(IAmphoraeService amphoraeService,
                                  IEntityStore<DataRequestModel> dataRequestStore,
                                  IEntityStore<OrganisationModel> organisationStore)
        {
            this.amphoraeService = amphoraeService;
            this.dataRequestStore = dataRequestStore;
            this.organisationStore = organisationStore;
        }

        // this just returns all the amphora is memory
        public async Task<EntitySearchResult<AmphoraModel>> SearchAmphora(string searchText, Models.Search.SearchParameters parameters)
        {
            var entities = new List<AmphoraModel>();

            if (string.IsNullOrEmpty(parameters.OrganisationId))
            {
                var res = await amphoraeService.AmphoraStore.QueryAsync(
                    a => a.Name.Contains(searchText) || a.Description.Contains(searchText), parameters.Skip ?? 0, parameters.Top ?? 99);
                entities.AddRange(res);
            }
            else
            {
                // filter by organisation
                var res = await amphoraeService.AmphoraStore.QueryAsync(
                    a =>
                        (a.Name.Contains(searchText) || a.Description.Contains(searchText))
                        && a.OrganisationId == parameters.OrganisationId,
                    parameters.Skip ?? 0, parameters.Top ?? 99);
                entities.AddRange(res);
            }

            return new EntitySearchResult<AmphoraModel>(entities);
        }

        public async Task<long?> SearchAmphoraCount(string searchText, SearchParameters parameters)
        {
            var res = await SearchAmphora(searchText, parameters);
            return res.Results.Count;
        }

        public async Task<EntitySearchResult<T>> SearchAsync<T>(string searchText, SearchParameters parameters) where T : ISearchable
        {
            searchText ??= ""; // ensure searchText not null

            if (typeof(T) == typeof(AmphoraModel))
            {
                var entities = new List<AmphoraModel>();

                var res = await amphoraeService.AmphoraStore.QueryAsync(
                    a => a.Name.Contains(searchText) || a.Description.Contains(searchText), parameters?.Skip ?? 0, parameters?.Top ?? 99);
                entities.AddRange(res);
                var result = new EntitySearchResult<T>(entities.ToList().Cast<T>());

                if (parameters.IncludeTotalResultCount)
                {
                    result.Count = await amphoraeService.AmphoraStore.CountAsync(a =>
                        a.Name.Contains(searchText) || a.Description.Contains(searchText));
                }

                return result;
            }
            else if (typeof(T) == typeof(DataRequestModel))
            {
                var entities = new List<DataRequestModel>();

                var res = await dataRequestStore.QueryAsync(
                    a => a.Name.Contains(searchText) || a.Description.Contains(searchText), parameters?.Skip ?? 0, parameters?.Top ?? 99);
                entities.AddRange(res);
                var result = new EntitySearchResult<T>(entities.ToList().Cast<T>());
                if (parameters.IncludeTotalResultCount)
                {
                    result.Count = await dataRequestStore.CountAsync(a =>
                        a.Name.Contains(searchText) || a.Description.Contains(searchText));
                }

                return result;
            }
            else if (typeof(T) == typeof(OrganisationModel))
            {
                var entities = new List<OrganisationModel>();

                var res = await organisationStore.QueryAsync(
                    a => a.Name.Contains(searchText) || a.About.Contains(searchText), parameters?.Skip ?? 0, parameters?.Top ?? 99);
                entities.AddRange(res);
                var result = new EntitySearchResult<T>(entities.ToList().Cast<T>());
                if (parameters.IncludeTotalResultCount)
                {
                    result.Count = await organisationStore.CountAsync(a =>
                        a.Name.Contains(searchText) || a.About.Contains(searchText));
                }

                return result;
            }
            else
            {
                throw new System.ArgumentException($"{typeof(T)} is not searchable");
            }
        }

        public Task<bool> TryIndex()
        {
            return Task.FromResult(true);
        }
    }
}