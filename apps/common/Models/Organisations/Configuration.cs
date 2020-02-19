namespace Amphora.Common.Models.Organisations
{
    public class Configuration
    {
        public int? MaximumSignals { get; set; }

        public int GetMaximumSignals() => MaximumSignals ?? 10;
    }
}