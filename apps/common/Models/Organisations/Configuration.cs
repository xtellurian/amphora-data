namespace Amphora.Common.Models.Organisations
{
    public class Configuration
    {
        private const int DefaultMaximumSignals = 10;
        public int? MaximumSignals { get; set; } = DefaultMaximumSignals;
        public int GetMaximumSignals() => MaximumSignals ?? DefaultMaximumSignals;
    }
}