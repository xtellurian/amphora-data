using Amphora.Common.Models.Amphorae;

namespace Amphora.Common.Models.Signals
{
    public class AmphoraSignalModel
    {
        // pure navigtion clas for many-to-many
        public string AmphoraId { get; set; }
        public AmphoraModel Amphora { get; set; }
        public string SignalId { get; set; }
        public SignalModel Signal { get; set; }
    }
}