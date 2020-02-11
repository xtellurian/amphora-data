using System.Threading.Tasks;
using Amphora.Common.Models.Purchases;

namespace Amphora.Api.Contracts
{
    public interface ICommissionTrackingService
    {
        Task<CommissionModel> TrackCommissionAsync(PurchaseModel purchase, double? commissionAmount);
    }
}