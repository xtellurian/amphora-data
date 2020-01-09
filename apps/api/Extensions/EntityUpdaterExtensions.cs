using System.Collections.ObjectModel;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Common.Models.Amphorae;

namespace Amphora.Api.Extensions
{
    public static class EntityUpdaterExtensions
    {
        public static AmphoraModel UpdateProperties(this AmphoraModel entity, AmphoraDto dto)
        {
            entity.Name = dto.Name;
            entity.Price = dto.Price;
            SetLabels(entity, dto);
            return entity;
        }

        public static AmphoraModel UpdateProperties(this AmphoraModel entity, AmphoraExtendedDto dto)
        {
            UpdateProperties(entity, dto as AmphoraDto);
            entity.TermsAndConditionsId = dto.TermsAndConditionsId;
            entity.Description = dto.Description;
            if (dto.Lat.HasValue && dto.Lon.HasValue)
            {
                entity.GeoLocation = new GeoLocation(dto.Lon.Value, dto.Lat.Value);
            }

            return entity;
        }

        private static void SetLabels(AmphoraModel entity, AmphoraDto dto)
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