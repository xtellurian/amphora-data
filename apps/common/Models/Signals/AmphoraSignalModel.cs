using Amphora.Common.Models.Amphorae;

namespace Amphora.Common.Models.Signals
{
    public class AmphoraSignalModel
    {
        public AmphoraSignalModel()
        { 
        }
        public AmphoraSignalModel(AmphoraModel amphora, SignalModel signal )
        {
            AmphoraId = amphora.Id;
            Amphora = amphora;
            SignalId = signal.Id;
            Signal = signal;
        }
        public AmphoraSignalModel(string amphoraId, AmphoraModel amphora, string signalId, SignalModel signal )
        {
            AmphoraId = amphoraId;
            Amphora = amphora;
            SignalId = signalId;
            Signal = signal;
        }
        // pure navigtion clas for many-to-many
        public string AmphoraId { get; set; } = null!;
        public virtual AmphoraModel Amphora { get; set; } = null!;
        public string SignalId { get; set; } = null!;
        public virtual SignalModel Signal { get; set; } = null!;
    }
}