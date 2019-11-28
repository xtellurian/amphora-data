namespace Amphora.Api.Options
{
    public class AmphoraManagementOptions
    {
        public bool? SoftDelete { get; set; } = false;
        public int? DeletedTimeToLive { get; set; } = 48 * 60 * 60;
        public string FeaturedAmphoraId { get; set; }
    }
}