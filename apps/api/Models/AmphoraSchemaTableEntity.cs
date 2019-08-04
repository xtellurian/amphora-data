using Microsoft.Azure.Cosmos.Table;

namespace Amphora.Api.Models
{
    public class AmphoraSchemaTableEntity : TableEntity
    {
        public string Id { get; set; }
        public string JsonSchema { get; set; }
    }
}