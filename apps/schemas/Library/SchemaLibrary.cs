using Newtonsoft.Json.Schema.Generation;
using Amphora.Common.Models;

namespace Amphora.Schemas.Library
{
    public class SchemaLibrary
    {
        public class BasicSensor
        {
            public string Id { get; set; }
            public double Value { get; set; }
        }

        public class TestSchema : AmphoraSchema
        {
            private const string jsonSchema = "";

            public TestSchema()
            {
                var generator = new JSchemaGenerator();
                this.JsonSchema = generator.Generate(typeof(BasicSensor));
                this.Id = nameof(TestSchema);
            }
        }
    }
}