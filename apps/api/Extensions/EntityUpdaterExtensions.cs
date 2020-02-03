using System.Collections.ObjectModel;
using System.Linq;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Signals;

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

        public static AmphoraModel UpdateProperties(this AmphoraModel entity, DetailedAmphora dto)
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

        public static void EnsureV2Signals(this AmphoraModel amphora)
        {
            if (amphora.V2Signals == null)
            {
                amphora.V2Signals = new Collection<SignalV2>();
            }

            var signals = amphora.Signals; // TODO: remove after V2 migrate

            foreach (var s in signals)
            {
                var v2 = new SignalV2(s.Signal.Property, s.Signal.ValueType);
                if (!amphora.V2Signals.Any(_ => _.Property == s.Signal.Property && _.ValueType == s.Signal.ValueType))
                {
                    amphora.V2Signals.Add(new SignalV2(s.Signal.Property, s.Signal.ValueType));
                }
            }
        }

        public static void RemoveV1Signals(this AmphoraModel amphora)
        {
            if (amphora.V2Signals == null)
            {
                amphora.V2Signals = new Collection<SignalV2>();
            }

            amphora.Signals = new Collection<AmphoraSignalModel>(); // TODO: remove after V2 migrate
        }
    }
}