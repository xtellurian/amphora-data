using Amphora.Common.Models.Amphorae;

namespace Amphora.Common.Models.Signals
{
    public class AmphoraSignalModel
    {
        // pure navigtion clas for many-to-many
        public string AmphoraId { get; set; } = null!;
        public virtual AmphoraModel Amphora { get; set; } = null!;
        public string SignalId { get; set; } = null!;
        public virtual SignalModel Signal { get; set; } = null!;
    }
}