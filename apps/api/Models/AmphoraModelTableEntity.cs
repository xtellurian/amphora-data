using Amphora.Common.Models;
using Microsoft.Azure.Cosmos.Table;

namespace Amphora.Api.Models
{

    public class AmphoraModelTableEntity : TableEntity
    {
        public AmphoraModelTableEntity()
        {
        }
        public string Id { get; set; }
        public string SchemaId { get; set; }
        public bool Bounded { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }

    }
}