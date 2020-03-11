namespace Amphora.Common.Models.Options
{
    public class AzureEventGridTopicOptions
    {
        // EventGrid--AppTopic--SecondaryKey
        public string? Endpoint { get; set; }
        public string? PrimaryKey { get; set; }
        public string? SecondaryKey { get; set; }
    }
}