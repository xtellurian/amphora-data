namespace Amphora.Api.Options
{
    public class FeatureFlagOptions
    {
        public static string Enabled => "enabled";
        public static string Disabled => "disabled";
        public bool IsSignalsEnabled => string.Equals(Signals, Enabled);
        public string Signals {get; set;}
    }
}
