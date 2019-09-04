namespace Amphora.Api.Contracts
{
    public interface IAmphoraeService
    {
        System.Threading.Tasks.Task<Common.Models.Amphora> CreateAsync(Common.Models.Amphora model, System.Security.Claims.ClaimsPrincipal creator);
    }
}