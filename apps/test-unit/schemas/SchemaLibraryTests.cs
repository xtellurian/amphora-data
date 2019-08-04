using System;
using System.Collections.Generic;
using Amphora.Common.Models;
using Amphora.Schemas.Library;
using Xunit;

namespace Amphora.Tests.Unit
{
    public class SchemaLibraryTests
    {
        private List<string> knownSchemas = new List<string>
        {
            "IdValue"
        };
        [Fact]
        public void IsInLibrary()
        {
            var library = new SchemaLibrary();
            foreach (var schemaId in knownSchemas)
            {
                Assert.True(library.IsInLibrary(schemaId));
                var schema = library.Load(schemaId);
                Assert.Equal(schemaId, schema.Id);
                Assert.NotNull(schema.JsonSchema);
            }
        }
        [Fact]
        public void ThrowsWhenNotInLibrary()
        {
            var library = new SchemaLibrary();
            var weirdId = "sldgnskbiksjngsldkn";
            Assert.False(library.IsInLibrary(weirdId));
            Assert.Throws<ArgumentException>(() => library.Load(weirdId));
        }
    }
}