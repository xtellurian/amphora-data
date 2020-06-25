namespace Amphora.Api.Models.Dtos.Geo
{
    public partial class Summary
    {
        public string Query { get; set; }
        public string QueryType { get; set; }
        public long NumResults { get; set; }
        public long TotalResults { get; set; }
        public long FuzzyLevel { get; set; }
    }
}