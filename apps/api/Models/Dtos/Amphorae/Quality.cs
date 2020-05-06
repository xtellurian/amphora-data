using System.ComponentModel.DataAnnotations;

namespace Amphora.Api.Models.Dtos.Amphorae
{
    public class Quality
    {
        public Quality()
        { }

        public Quality(QualityLevels? accuracy, QualityLevels? completeness, QualityLevels? granularity, QualityLevels? reliability)
        {
            Accuracy = accuracy;
            Completeness = completeness;
            Granularity = granularity;
            Reliability = reliability;
        }

        [Range(1, 4, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public QualityLevels? Accuracy { get; set; }
        [Range(1, 4, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public QualityLevels? Completeness { get; set; }
        [Range(1, 4, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public QualityLevels? Granularity { get; set; }
        [Range(1, 4, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public QualityLevels? Reliability { get; set; }
    }

    public enum QualityLevels
    {
        Low = 1,
        Medium = 2,
        High = 3,
        Perfect = 4,
    }
}