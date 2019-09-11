using System.IO;
using System.Threading.Tasks;

namespace Amphora.Api.Contracts
{
    public interface IBinaryAmphoraFiller
    {
        Task Fill(Amphora.Common.Models.AmphoraModel amphora, Stream data);
        bool IsAmphoraSupported(Common.Models.AmphoraModel amphora);
    }
    public interface IBinaryAmphoraDrinker
    {
        Task<Stream> Drink(Amphora.Common.Models.AmphoraModel amphora);
        bool IsAmphoraSupported(Amphora.Common.Models.AmphoraModel amphora);
    }
}