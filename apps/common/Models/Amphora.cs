using Amphora.Common.Contracts;

namespace Amphora.Common.Models
{
    public class Amphora : DataEntity, IDataEntity
    {
        public Amphora()
        {
            // set some defaults
            ContentType = ContentTypes.OctetStream;
        }
        public string ContentType { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
    }
}