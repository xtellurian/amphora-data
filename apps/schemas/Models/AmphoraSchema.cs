using common.Contracts;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace schema.Models
{
    public class AmphoraSchema : IAmphoraEntity
    {
        public AmphoraSchema() { }
        public AmphoraSchema(string jsonSchema)
        {
            this.JsonSchema = JSchema.Parse(jsonSchema);
        }
        public AmphoraSchema(JSchema jSchema)
        {
            this.JsonSchema = jSchema;
        }
        
        public string Id { get; set; }
        public JSchema JsonSchema { get; set; }

        public bool IsValid(JObject jObj)
        {
            return jObj.IsValid(JsonSchema);
        }

    }
}