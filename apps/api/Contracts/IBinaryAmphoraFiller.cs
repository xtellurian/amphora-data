using System.IO;
using System.Threading.Tasks;

namespace Amphora.Api.Contracts
{
    public interface IBinaryAmphoraFiller
    {
        Task Fill(Amphora.Common.Models.Amphora amphora, Stream data);
        bool IsAmphoraSupported(Common.Models.Amphora amphora);
    }
    public interface IBinaryAmphoraDrinker
    {
        Task<Stream> Drink(Amphora.Common.Models.Amphora amphora);
        bool IsAmphoraSupported(Amphora.Common.Models.Amphora amphora);
    }
}