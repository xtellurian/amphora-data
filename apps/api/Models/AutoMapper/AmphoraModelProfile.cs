using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Organisations;
using AutoMapper;

namespace Amphora.Api.Models.AutoMapper
{
    public class AmphoraModelProfile : Profile
    {
        public AmphoraModelProfile()
        {
            // this is to extend a base class into its extended version. 
            // Generally only used in the InMemory stores.
            CreateMap<AmphoraModel, AmphoraExtendedModel>()
            .IncludeAllDerived()
            .ForMember(o => o.Description, p => p.Ignore())
            .ForMember(o => o.GeoLocation, p => p.Ignore());

            
            CreateMap<AmphoraModel, AmphoraSecurityModel>()
            .IncludeAllDerived()
            .ForMember(o => o.HasPurchased, p => p.Ignore());
        }
    }
}
