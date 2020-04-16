using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Purchases;
using Amphora.Common.Models.Users;
using Newtonsoft.Json;

namespace Amphora.Common.Models.Amphorae
{
    public class AmphoraModel : EntityBase, ISearchable
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
            Price = price.HasValue ? Math.Round(price.Value, 2) : 0;
            OrganisationId = organisationId;
        }

        public AmphoraModel(
            string name,
            string description,
            double? price,
            string organisationId,
            string? createdById = null,
            string? termsAndConditionsId = null) : this(name, description, price, organisationId)
        {
            CreatedById = createdById;
            TermsAndConditionsId = termsAndConditionsId;
        }

        public string Name { get; set; }
        public bool? IsPublic { get; set; }
        public double? Price { get; set; }
        public int? PurchaseCount { get; set; }
        public string Description { get; set; }
        public GeoLocation? GeoLocation { get; set; }
        public Dictionary<string, AttributeStore>? FileAttributes { get; set; } = new Dictionary<string, AttributeStore>();
        public virtual ICollection<SignalV2>? V2Signals { get; set; } = new Collection<SignalV2>();
        public virtual ICollection<Label>? Labels { get; set; } = new Collection<Label>();

        // navigation
        public string OrganisationId { get; set; }
        public virtual OrganisationModel Organisation { get; set; } = null!;
        public string? CreatedById { get; set; }
        public virtual ApplicationUserDataModel? CreatedBy { get; set; }
        public virtual ICollection<PurchaseModel> Purchases { get; set; } = new Collection<PurchaseModel>();
        public virtual AmphoraAccessControlModel AccessControl { get; set; } = new AmphoraAccessControlModel();
        public string? TermsAndConditionsId { get; set; }
        public TermsAndConditionsModel? TermsAndConditions => this.Organisation?.TermsAndConditions?.FirstOrDefault(o => o.Id == TermsAndConditionsId);

        // methods

        /// <summary>
        /// Method checks IsPublic is not null and is true.
        /// </summary>
        public bool Public()
        {
            return this.IsPublic.HasValue && this.IsPublic.Value;
        }

        /// <summary>
        /// Useful incase a collection is null.
        /// </summary>
        public ICollection<T> GetSafe<T>(Func<AmphoraModel, ICollection<T>> selector)
        {
            return selector(this) ?? new Collection<T>();
        }

        /// <summary>
        /// Useful incase a collection is null.
        /// </summary>
        public bool TryAddSignal(SignalV2 signal, out string message)
        {
            if (this.V2Signals.Any(s => s.Property == signal.Property))
            {
                message = $"Duplicate property name: {signal.Property}";
                return false;
            }

            if (!CanAddNewSignal())
            {
                // then don't add the signal
                var limit = GetSignalsLimit();
                message = $"You've reached your limit of {limit} signals";
                return false;
            }

            V2Signals ??= new Collection<SignalV2>();
            this.V2Signals.Add(signal);
            message = string.Empty;
            return true;
        }

        public bool CanAddNewSignal()
        {
            var limit = GetSignalsLimit();
            if (this.V2Signals?.Count >= limit)
            {
                return false;
            }

            return true;
        }

        private int? GetSignalsLimit()
        {
            if (this.Organisation == null) { return new Organisations.Configuration().GetMaximumSignals(); }
            this.Organisation.Configuration ??= new Organisations.Configuration(); // just create one if its null
            var limit = this.Organisation.Configuration?.GetMaximumSignals();
            return limit;
        }
    }
}