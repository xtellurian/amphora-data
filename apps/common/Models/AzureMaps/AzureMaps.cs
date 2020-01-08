using System.Collections.Generic;
// this was generated using QuickType
namespace Amphora.Common.Models.AzureMaps
{
    public partial class FuzzySearchResponse
    {
        public Summary? Summary { get; set; }
        public List<Result> Results { get; set; } = new List<Result>();
    }

    public partial class Result
    {
        public string? Type { get; set; }
        public string? Id { get; set; }
        public double Score { get; set; }
        public Address? Address { get; set; }
        public Position? Position { get; set; }
        public Viewport? Viewport { get; set; }
        public List<EntryPoint> EntryPoints { get; set; } = new List<EntryPoint>();
        public string? Info { get; set; }
        public Poi? Poi { get; set; }
    }

    public partial class Address
    {
        public string? StreetNumber { get; set; }
        public string? StreetName { get; set; }
        public string? MunicipalitySubdivision { get; set; }
        public string? Municipality { get; set; }
        public string? CountrySecondarySubdivision { get; set; }
        public string? CountrySubdivision { get; set; }
        public string? PostalCode { get; set; }
        public string? CountryCode { get; set; }
        public string? Country { get; set; }
        public string? CountryCodeIso3 { get; set; }
        public string? FreeformAddress { get; set; }
        public string? LocalName { get; set; }
    }

    public partial class EntryPoint
    {
        public string? Type { get; set; }
        public Position? Position { get; set; }
    }

    public partial class Position
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
    }

    public partial class Poi
    {
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public List<CategorySet> CategorySet { get; set; } = new List<CategorySet>();
        public List<string> Categories { get; set; } = new List<string>();
        public List<Classification> Classifications { get; set; } = new List<Classification>();
    }

    public partial class CategorySet
    {
        public long Id { get; set; }
    }

    public partial class Classification
    {
        public string? Code { get; set; }
        public List<Name> Names { get; set; } = new List<Name>();
    }

    public partial class Name
    {
        public string? NameLocale { get; set; }
        public string? NameName { get; set; }
    }

    public partial class Viewport
    {
        public Position? TopLeftPoint { get; set; }
        public Position? BtmRightPoint { get; set; }
    }

    public partial class Summary
    {
        public string? Query { get; set; }
        public string? QueryType { get; set; }
        public long QueryTime { get; set; }
        public long NumResults { get; set; }
        public long Offset { get; set; }
        public long TotalResults { get; set; }
        public long FuzzyLevel { get; set; }
    }
}