using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using schemas.Library;
using Xunit;

namespace test_unit
{
    public class SchemaValidation
    {
        private const string randomJson = @"{'thing': 'value' }";
        [Fact]
        public void TestStandardSchemaValidation()
        {
            var schema = new SchemaLibrary.TestSchema();
            Assert.NotNull(schema.Id);
            var randomJObject = JObject.Parse(randomJson);
            var result = schema.IsValid(randomJObject);
            Assert.False(result);
        }
    }
}
