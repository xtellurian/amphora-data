using Amphora.Common.Models.Organisations.Accounts;
using AutoMapper;

namespace Amphora.Api.Models.AutoMapper
{
    public class InvoicesProfile : Profile
    {
        public InvoicesProfile()
        {
            CreateMap<Invoice, Dtos.Accounts.Invoice>()
                .ForMember(m => m.OrganisationId, a => a.MapFrom(b => b.Account.OrganisationId));
            CreateMap<Transaction, Dtos.Accounts.Transaction>();
        }
    }
}