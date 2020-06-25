namespace Amphora.Api.Models.Dtos.Geo
{
    public partial class Result
    {
        public string Id { get; set; }
        public double Score { get; set; }
        public Address Address { get; set; }
        public Position Position { get; set; }
        public string Info { get; set; }
    }
}