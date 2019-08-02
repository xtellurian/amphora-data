using api.Contracts;
using Newtonsoft.Json.Schema;

namespace api.Models
{
    public class AmphoraSchema : IAmphoraEntity
    {
        public string Id { get; set; }
        public string JsonSchema { get; set; }
        public JSchema LoadJSchema()
        {
            return JSchema.Parse(this.JsonSchema);
        }
    }
}