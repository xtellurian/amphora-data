using AutoMapper;

namespace Amphora.Api.Models.AutoMapper
{
    public class FuzzySearchResponseProfile : Profile
    {
        public FuzzySearchResponseProfile()
        {
            CreateMap<Amphora.Common.Models.AzureMaps.FuzzySearchResponse, Dtos.Geo.FuzzySearchResponse>();
            CreateMap<Amphora.Common.Models.AzureMaps.Summary, Dtos.Geo.Summary>();
            CreateMap<Amphora.Common.Models.AzureMaps.Result, Dtos.Geo.Result>();
            CreateMap<Amphora.Common.Models.AzureMaps.Address, Dtos.Geo.Address>();
            CreateMap<Amphora.Common.Models.AzureMaps.Position, Dtos.Geo.Position>();
        }
    }
}
