using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Api.Models.Dtos.Platform;
using Amphora.Api.Models.Dtos.Search;
using Amphora.Api.Models.Search;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Signals;

namespace Amphora.Api.Extensions
{
    public static class EntityUpdaterExtensions
    {
        public static AmphoraModel UpdateProperties(this AmphoraModel entity, BasicAmphora dto)
        {
            entity.Name = dto.Name;
            entity.Price = dto.Price;
            SetLabels(entity, dto);
            return entity;
        }

        public static AmphoraModel UpdateProperties(this AmphoraModel entity, EditAmphora dto)
        {
            UpdateProperties(entity, dto as BasicAmphora);
            entity.TermsOfUseId = dto.TermsOfUseId;
            entity.Description = dto.Description;
            if (dto.Lat.HasValue && dto.Lon.HasValue)
            {
                entity.GeoLocation = new GeoLocation(dto.Lon.Value, dto.Lat.Value);
            }

            return entity;
        }

        public static SearchParameters ToSearchParameters(this AmphoraSearchQueryOptions queryParameters)
        {
            queryParameters ??= new AmphoraSearchQueryOptions();
            var parameters = queryParameters.ToPaginatedSearchParameters();
            var labelsArray = queryParameters.Labels?.Split(',')?.ToList();
            if (labelsArray != null && labelsArray.Count > 0)
            {
                parameters = parameters.FilterByLabel<AmphoraModel>(new List<Label>(labelsArray.Select(_ => new Label(_))));
            }

            if (queryParameters.Lat != null && queryParameters.Lon != null)
            {
                parameters.WithGeoSearch<AmphoraModel>(queryParameters.Lat.Value, queryParameters.Lon.Value, queryParameters.Dist ?? 50);
            }

            if (!string.IsNullOrEmpty(queryParameters.OrgId))
            {
                parameters = parameters.FilterByOrganisation<AmphoraModel>(queryParameters.OrgId);
            }

            return parameters;
        }

        public static SearchParameters ToSearchParameters(this DataRequestSearchQueryOptions queryParameters)
        {
            queryParameters ??= new DataRequestSearchQueryOptions();
            var parameters = queryParameters.ToPaginatedSearchParameters();

            if (queryParameters.Lat != null && queryParameters.Lon != null)
            {
                parameters.WithGeoSearch<AmphoraModel>(queryParameters.Lat.Value, queryParameters.Lon.Value, queryParameters.Dist ?? 50);
            }

            if (!string.IsNullOrEmpty(queryParameters.OrgId))
            {
                parameters = parameters.FilterByOrganisation<AmphoraModel>(queryParameters.OrgId);
            }

            return parameters;
        }

        private static SearchParameters ToPaginatedSearchParameters(this PaginatedResponse paginated)
        {
            var parameters = new SearchParameters();
            parameters.Skip = paginated.Skip;
            parameters.Top = paginated.Take;
            parameters.IncludeTotalResultCount = true;
            return parameters;
        }

        private static void SetLabels(AmphoraModel entity, BasicAmphora dto)
        {
            if (entity.Labels == null) { entity.Labels = new Collection<Label>(); }
            else { entity.Labels.Clear(); }
            foreach (var l in dto.GetLabels())
            {
                entity.Labels.Add(l);
            }
        }
    }
}