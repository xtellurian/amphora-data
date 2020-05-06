namespace Amphora.Common.Models.Amphorae
{
    public class DataQuality
    {
        public int? Completeness { get; set; }
        public int? Accuracy { get; set; }
        public int? Reliability { get; set; }
        public int? Granularity { get; set; }

        public string Description(int? metric)
        {
            switch (metric)
            {
                case null:
                    return "Not Given";
                case 1:
                    return "Low";
                case 2:
                    return "Medium";
                case 3:
                    return "High";
                case 4:
                    return "Perfect";
                default:
                    return "Unknown";
            }
        }
    }
}