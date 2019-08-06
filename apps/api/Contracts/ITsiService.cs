namespace Amphora.Api.Contracts
{
    public interface ITsiService
    {
        System.Threading.Tasks.Task<string> GetAccessTokenAsync();
    }
}