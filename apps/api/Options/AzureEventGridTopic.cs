namespace Amphora.Api.Options
{
    public class AzureEventGridTopic
    {
        // EventGrid--AppTopic--SecondaryKey
        public string Endpoint { get; set; }
        public string PrimaryKey { get; set; }
        public string SecondaryKey { get; set; }
    }
}