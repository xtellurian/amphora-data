namespace Amphora.Api.Options
{
    public class FeatureFlagOptions
    {
        public static string Enabled => "enabled";
        public static string Disabled => "disabled";
        public bool IsSignalsEnabled => string.Equals(Signals, Enabled);
        public bool IsInvoicesEnabled => string.Equals(Invoices, Enabled);
        public string Signals { get; set; }
        public string Invoices { get; set; }
    }
}
