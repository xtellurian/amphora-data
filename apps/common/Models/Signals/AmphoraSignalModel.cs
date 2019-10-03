using Amphora.Common.Models.Amphorae;

namespace Amphora.Common.Models.Signals
{
    public class AmphoraSignalModel
    {
        // pure navigtion clas for many-to-many
        public string AmphoraId { get; set; }
        public virtual AmphoraModel Amphora { get; set; }
        public string SignalId { get; set; }
        public virtual SignalModel Signal { get; set; }
    }
}