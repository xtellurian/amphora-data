namespace Amphora.Common.Models.AzureMaps
{
    public class IpAddressToLocationResult
    {
        public string? IpAddress { get; set; }
        public CountryRegion? CountryRegion { get; set; }
    }

    public class CountryRegion
    {
        public string? IsoCode { get; set; }
    }
}