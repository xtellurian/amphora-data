using System.Threading.Tasks;
using Amphora.Common.Models.Amphorae;

namespace Amphora.Api.Contracts
{
    public interface IQualityEstimatorService
    {
        Task<EnrichedDataQuality> GenerateDataQualitySummaryAsync(AmphoraModel amphora);
    }
}