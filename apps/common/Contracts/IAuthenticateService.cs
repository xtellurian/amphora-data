using System.Threading.Tasks;
using Amphora.Common.Models.Platform;

namespace Amphora.Common.Contracts
{
    public interface IAuthenticateService
    {
        Task<(bool success, string token)> GetToken(LoginRequest request);
    }
}