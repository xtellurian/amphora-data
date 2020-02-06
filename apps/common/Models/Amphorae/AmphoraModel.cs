using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Purchases;
using Amphora.Common.Models.Signals;
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
        public Dictionary<string, MetaDataStore>? FilesMetaData { get; set; } = new Dictionary<string, MetaDataStore>();
        public virtual ICollection<SignalV2>? V2Signals { get; set; } = new Collection<SignalV2>();
        public virtual ICollection<Label>? Labels { get; set; } = new Collection<Label>();

        // navigation
        public string OrganisationId { get; set; }
        public virtual OrganisationModel Organisation { get; set; } = null!;
        public string? CreatedById { get; set; }
        public virtual ApplicationUser? CreatedBy { get; set; }
        // [Obsolete]
        public virtual ICollection<AmphoraSignalModel> Signals { get; set; } = new Collection<AmphoraSignalModel>();
        public virtual ICollection<PurchaseModel> Purchases { get; set; } = new Collection<PurchaseModel>();
        public string? TermsAndConditionsId { get; set; }
        public TermsAndConditionsModel? TermsAndConditions => this.Organisation?.TermsAndConditions?.FirstOrDefault(o => o.Id == TermsAndConditionsId);

        // methods

        /// <summary>
        /// Method checks IsPublic is not null and is true
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
    }
}