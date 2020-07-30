using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Search;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.DataRequests;
using Amphora.Common.Models.Organisations;
using Microsoft.EntityFrameworkCore;

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

        public async Task<EntitySearchResult<T>> SearchAsync<T>(string searchText, SearchParameters parameters) where T : ISearchable
        {
            searchText ??= ""; // ensure searchText not null

            if (typeof(T) == typeof(AmphoraModel))
            {
                var entities = new List<AmphoraModel>();

                IQueryable<AmphoraModel> query;

                if (string.IsNullOrEmpty(parameters.OrganisationId))
                {
                    query = (await amphoraeService.AmphoraStore.Query(
                        a => a.Name.Contains(searchText) || a.Description.Contains(searchText))
                        .ToListAsync())
                        .AsQueryable();
                }
                else
                {
                    query = (await amphoraeService.AmphoraStore.Query(
                        a => (a.Name.Contains(searchText) || a.Description.Contains(searchText))
                        && a.OrganisationId == parameters.OrganisationId)
                        .ToListAsync())
                        .AsQueryable();
                }

                if (parameters.Labels.Any())
                {
                    query = query
                        .Where(a => a.Labels.Any(a => parameters.Labels.Any(b => b.Name == a.Name)));
                }

                entities.AddRange(query.Skip(parameters.Skip ?? 0).Take(parameters.Top ?? 64).ToList());
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