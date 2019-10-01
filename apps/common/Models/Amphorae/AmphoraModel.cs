using System.Collections.Generic;
using Amphora.Common.Models.Domains;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Transactions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Amphora.Common.Models.Amphorae
{
    public class AmphoraModel : Entity
    {
        public AmphoraModel()
        {
            Transactions = new List<TransactionModel>();
        }

        public string OrganisationId { get; set; }
        public OrganisationModel Organisation { get; set; }
        public string Name { get; set; }
        public bool IsPublic { get; set; }
        public double? Price { get; set; }
        public string Description { get; set; }
        public GeoLocation GeoLocation { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public DomainId DomainId { get; set; }
        public List<TransactionModel> Transactions { get; set; }

    }
}