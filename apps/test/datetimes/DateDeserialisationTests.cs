using System;
using Amphora.Common.Models;
using Amphora.Common.Models.Domains.Agriculture;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Unit
{
    public class DateDeserialisationTests
    {
        [Theory]
        [InlineData("{ 't': '5/1/94'}", false)]
        [InlineData("{ 't': 'bad'}", true)]
        public void DeserializeToAgDatum(string json, bool throws)
        {
            if (throws)
            {
                Assert.Throws<Newtonsoft.Json.JsonReaderException>(() => JsonConvert.DeserializeObject<AgDatum>(json));
            }
            else
            {
                var datum = JsonConvert.DeserializeObject<AgDatum>(json);
            }
        }
    }
}