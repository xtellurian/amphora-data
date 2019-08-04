using System;
using Newtonsoft.Json.Linq;
using Amphora.Schemas.Library;
using Xunit;

namespace Amphora.Tests.Unit
{
    public class SchemaValidation
    {
        private const string randomJson = @"{'thing': 'value' }";
        private const string idValueJson = @"{'id': '1234', 'value': 1234 }";
        [Fact]
        public void IdValueSchemaInvalid()
        {
            var schema = new SchemaLibrary.IdValueSchema();
            Assert.NotNull(schema.Id);
            var randomJObject = JObject.Parse(randomJson);
            var result = schema.IsValid(randomJObject);
            Assert.False(result);
        }

        [Fact]
        public void IdValueSchemaValid()
        {
            var schema = new SchemaLibrary.IdValueSchema();
            Assert.NotNull(schema.Id);
            var jObj = JObject.Parse(idValueJson);
            Assert.True(schema.IsValid(jObj));
        }
    }
}
