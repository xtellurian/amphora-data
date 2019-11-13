namespace Amphora.Common.Configuration.Options
{
    public class CosmosOptions
    {
        public string PrimaryKey { get; set; }
        public string SecondaryKey { get; set; }
        public string PrimaryReadonlyKey { get; set; }
        public string SeconaryReadonlyKey { get; set; }

        public string Endpoint { get; set; }
        public string Database { get; set; }
        public string Container { get; set; }
        public int? DefaultTimeToLive { get; set; }

        public string GenerateConnectionString(string key)
        {
            return $"AccountEndpoint={Endpoint};AccountKey={key};Database={Database}";
        }
    }
}