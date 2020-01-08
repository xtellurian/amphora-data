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
            entity.Labels = dto.GetLabels();
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
    }
}