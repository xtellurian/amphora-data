using Amphora.Common.Models;
using Microsoft.Azure.Cosmos.Table;

namespace Amphora.Api.Models
{

    public class AmphoraTableEntity : TableEntity
    {
        public AmphoraTableEntity()
        {
        }
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }

    }
}