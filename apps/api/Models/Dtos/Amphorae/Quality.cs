using System.ComponentModel.DataAnnotations;

namespace Amphora.Api.Models.Dtos.Amphorae
{
    /// <summary>
    /// Quality metrics for an Amphora, between 1 and 4.
    /// </summary>
    public class Quality
    {
        public Quality()
        { }

        public Quality(int? accuracy = null,
                       int? completeness = null,
                       int? granularity = null,
                       int? reliability = null)
        {
            Accuracy = accuracy;
            Completeness = completeness;
            Granularity = granularity;
            Reliability = reliability;
        }

        [Range(1, 4, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int? Accuracy { get; set; }
        [Range(1, 4, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int? Completeness { get; set; }
        [Range(1, 4, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int? Granularity { get; set; }
        [Range(1, 4, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int? Reliability { get; set; }
    }
}