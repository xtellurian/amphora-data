using System.Collections.Generic;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Signals;
using Amphora.Common.Models.Transactions;
using Amphora.Common.Models.Users;

namespace Amphora.Common.Models.Amphorae
{
    public class AmphoraModel : Entity
    {
        public AmphoraModel()
        {
            Transactions = new List<TransactionModel>();
        }

        public string Name { get; set; }
        public bool IsPublic { get; set; }
        public double? Price { get; set; }
        public string Description { get; set; }
        public GeoLocation GeoLocation { get; set; }

        // navigation
        public string OrganisationId { get; set; }
        public OrganisationModel Organisation { get; set; }
        public string CreatedById { get; set; }
        public ApplicationUser CreatedBy { get; set; }
        public List<AmphoraSignalModel> Signals {get;set;}
        public List<TransactionModel> Transactions { get; set; }

        // methods
        public void AddSignal(SignalModel signal)
        {
            if(Signals == null) Signals = new List<AmphoraSignalModel>();
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