namespace Amphora.Api.Models.Dtos.Activities
{
    public class AmphoraReference
    {
        public string AmphoraId { get; set; }
        public int? FilesConsumed { get; set; }
        public int? FilesProduced { get; set; }
        public int? SignalsConsumed { get; set; }
        public int? SignalsProduced { get; set; }
    }
}