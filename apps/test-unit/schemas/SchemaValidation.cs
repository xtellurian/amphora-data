using System;
using Newtonsoft.Json.Linq;
using Amphora.Schemas.Library;
using Xunit;

namespace Amphora.Tests.Unit
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
