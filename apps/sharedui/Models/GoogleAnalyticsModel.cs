namespace Amphora.SharedUI.Models
{
    public class GoogleAnalyticsModel
    {
        public GoogleAnalyticsModel(string trackingId)
        {
            if (string.IsNullOrEmpty(trackingId))
            {
                System.Console.WriteLine("Google Analytics Tracking Id not provided");
            }

            TrackingId = trackingId;
        }

        public string TrackingId { get; set; }
    }
}