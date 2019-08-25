using System.Threading.Tasks;
using Amphora.Api.Models;

namespace Amphora.Api.Contracts
{
    public interface IAuthenticateService
    {
        Task<(bool success, string token)> IsAuthenticated(TokenRequest request);
    }
}