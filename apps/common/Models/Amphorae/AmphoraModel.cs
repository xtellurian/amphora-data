using System.Collections.Generic;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Signals;
using Amphora.Common.Models.Purchases;
using Amphora.Common.Models.Users;
using System.Linq;

namespace Amphora.Common.Models.Amphorae
{
    public class AmphoraModel : Entity
    {
        public AmphoraModel()
        {
            Purchases = new List<PurchaseModel>();
        }

        public string Name { get; set; }
        public bool IsPublic { get; set; }
        public double? Price { get; set; }
        public string Description { get; set; }
        public GeoLocation GeoLocation { get; set; }

        // navigation
        public string OrganisationId { get; set; }
        public virtual OrganisationModel Organisation { get; set; }
        public string CreatedById { get; set; }
        public virtual ApplicationUser CreatedBy { get; set; }
        public virtual ICollection<AmphoraSignalModel> Signals { get; set; }
        public virtual ICollection<PurchaseModel> Purchases { get; set; }
        public string TermsAndConditionsId { get; set; }
        public TermsAndConditionsModel TermsAndConditions => this.Organisation?.TermsAndConditions?.FirstOrDefault(o => o.Name == TermsAndConditionsId);

        // methods
        public void AddSignal(SignalModel signal)
        {
            if (Signals == null) Signals = new List<AmphoraSignalModel>();
            if (Signals.Count >= 5)
            {
                throw new System.ArgumentException("Only 5 Signals per Amphora at this time");
            }
            Signals.Add(new AmphoraSignalModel
            {
                Amphora = this,
                AmphoraId = this.Id,
                Signal = signal,
                SignalId = signal.Id
            });
        }



    }
}