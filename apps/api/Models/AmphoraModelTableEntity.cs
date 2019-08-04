using Amphora.Common.Models;
using Microsoft.Azure.Cosmos.Table;

namespace Amphora.Api.Models
{

    public class AmphoraModelTableEntity : TableEntity
    {
        public AmphoraModelTableEntity()
        {
        }
        public AmphoraModelTableEntity(string partitionKey, string rowKey)
        {
            this.PartitionKey = partitionKey;
            this.RowKey = rowKey;

        }
        public string Id { get; set; }
        public AmphoraClass Class { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }

    }
}