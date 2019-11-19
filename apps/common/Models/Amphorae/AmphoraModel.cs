using System.Collections.Generic;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Signals;
using Amphora.Common.Models.Purchases;
using Amphora.Common.Models.Users;
using System.Linq;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace Amphora.Common.Models.Amphorae
{
    public class AmphoraModel : Entity
    {
        public AmphoraModel()
        {
            Name = null!;
            Description = null!;
            OrganisationId = null!;
        }
        [JsonConstructor]
        public AmphoraModel(string name, string description, double? price, string organisationId)
        {
            Name = name;
            Description = description;
            Price = price;
            OrganisationId = organisationId;
        }
        public AmphoraModel(
            string name,
            string description,
            double? price,
            string organisationId,
            string? createdById = null,
            string? termsAndConditionsId = null): this(name, description, price, organisationId)
        {
            CreatedById = createdById;
            TermsAndConditionsId = termsAndConditionsId;
        }

        public string Name { get; set; }
        public bool IsPublic { get; set; }
        public double? Price { get; set; }
        public string Description { get; set; }
        public GeoLocation? GeoLocation { get; set; }

        // navigation
        public string OrganisationId { get; set; }
        public virtual OrganisationModel Organisation { get; set; } = null!;
        public string? CreatedById { get; set; }
        public virtual ApplicationUser? CreatedBy { get; set; }
        public virtual ICollection<AmphoraSignalModel> Signals { get; set; } = new Collection<AmphoraSignalModel>();
        public virtual ICollection<PurchaseModel> Purchases { get; set; } = new Collection<PurchaseModel>();
        public string? TermsAndConditionsId { get; set; }
        public TermsAndConditionsModel? TermsAndConditions => this.Organisation?.TermsAndConditions?.FirstOrDefault(o => o.Id == TermsAndConditionsId);

        // methods
        public void AddSignal(SignalModel signal, int maxSignals = 7)
        {
            if (Signals == null) Signals = new List<AmphoraSignalModel>();
            if (Signals.Count >= maxSignals)
            {
                throw new System.ArgumentException($"Only {maxSignals} Signals per Amphora at this time");
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