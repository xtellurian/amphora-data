using Amphora.Common.Models.Amphorae;

namespace Amphora.Common.Models.Activities
{
    public class ActivityAmphoraReference
    {
        public ActivityAmphoraReference() { }
        public ActivityAmphoraReference(
            AmphoraModel amphora,
            int? filesConsumed = null,
            int? filesProduced = null,
            int? signalsConsumed = null,
            int? signalsProduced = null)
        {
            AmphoraId = amphora.Id;
            FilesConsumed = filesConsumed;
            FilesProduced = filesProduced;
            SignalsConsumed = signalsConsumed;
            SignalsProduced = signalsProduced;
        }

        public string? AmphoraId { get; set; }
        public int? FilesConsumed { get; set; }
        public int? FilesProduced { get; set; }
        public int? SignalsConsumed { get; set; }
        public int? SignalsProduced { get; set; }
    }
}