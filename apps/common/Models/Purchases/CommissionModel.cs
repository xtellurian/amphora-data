using Amphora.Common.Contracts;

namespace Amphora.Common.Models.Purchases
{
    public class CommissionModel : EntityBase, IEntity
    {
        public double? Amount { get; set; }
        public string? FromOrganisationId { get; set; }
        public string? FromOrganisationName { get; set; }
        public string? TriggeredByUsername { get; set; }
        public string? PurchaseModelId { get; set; }
        public virtual PurchaseModel? PurchaseModel { get; set; }
    }
}