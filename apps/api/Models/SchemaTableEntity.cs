using Microsoft.Azure.Cosmos.Table;

namespace Amphora.Api.Models
{
    public class SchemaTableEntity : TableEntity
    {
        public string Id { get; set; }
        public string JsonSchema { get; set; }
    }
}