namespace Amphora.Common.Models.Amphorae
{
    public class DataQualitySummary
    {
        public int? CountFiles { get; set; }
        public int? CountSignals { get; set; }
        public int? DaysSinceFilesLastUpdated { get; set; }
        public int? DaysSinceSignalsLastUpdated { get; set; }
    }
}