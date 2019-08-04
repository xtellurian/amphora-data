using Newtonsoft.Json.Schema.Generation;
using Amphora.Common.Models;
using Newtonsoft.Json.Serialization;

namespace Amphora.Schemas.Library
{
    public class SchemaLibrary
    {
        class IdValue
        {
            public string Id { get; set; }
            public double Value { get; set; }
        }

        public class IdValueSchema : AmphoraSchema
        {
            public IdValueSchema()
            {
                var generator = new JSchemaGenerator();
                generator.ContractResolver = new CamelCasePropertyNamesContractResolver();
                this.JsonSchema = generator.Generate(typeof(IdValue));
                this.Id = nameof(IdValue);
            }
        }
    }
}