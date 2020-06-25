namespace Amphora.Api.Models.Dtos.Geo
{
    public partial class Address
    {
        public string StreetNumber { get; set; }
        public string StreetName { get; set; }
        public string MunicipalitySubdivision { get; set; }
        public string Municipality { get; set; }
        public string CountrySecondarySubdivision { get; set; }
        public string CountrySubdivision { get; set; }
        public string PostalCode { get; set; }
        public string CountryCode { get; set; }
        public string Country { get; set; }
        public string CountryCodeIso3 { get; set; }
        public string FreeformAddress { get; set; }
        public string LocalName { get; set; }
    }
}