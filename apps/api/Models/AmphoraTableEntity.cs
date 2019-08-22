using Amphora.Common.Models;
using Microsoft.Azure.Cosmos.Table;

namespace Amphora.Api.Models
{

    public class AmphoraTableEntity : TableEntity
    {
        public AmphoraTableEntity()
        {
        }
        public string Description { get; set; }
        public string Id { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public double Price { get; set; }
        public string Title { get; set; }

    }
}
