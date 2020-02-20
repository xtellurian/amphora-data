using System.Collections.ObjectModel;
using System.Linq;
using Amphora.Api.Models.Dtos.Amphorae;
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

        public static AmphoraModel UpdateProperties(this AmphoraModel entity, DetailedAmphora dto)
        {
            UpdateProperties(entity, dto as BasicAmphora);
            entity.TermsAndConditionsId = dto.TermsAndConditionsId;
            entity.Description = dto.Description;
            if (dto.Lat.HasValue && dto.Lon.HasValue)
            {
                entity.GeoLocation = new GeoLocation(dto.Lon.Value, dto.Lat.Value);
            }

            return entity;
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