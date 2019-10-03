
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Amphora.Tests.Unit
{
    public class DateDeserialisationTests
    {
        [Theory]
        [InlineData("{ 't': '5/1/94'}", true)]
        [InlineData("{ 't': 'bad'}", false)]
        [InlineData("{ 'x': 'something else'}", false)]
        public void DeserialiseDateTime(string json, bool good)
        {
            var jObj = JObject.Parse(json);
            if (jObj.TryGetValue("t", out var token))
            {
                if (good)
                {
                    var time = token.Value<DateTime>();
                }
                else
                {
                    Assert.Throws<System.FormatException>(() => token.Value<DateTime>());
                }
            }
            else
            {
                Assert.False(good);
            }

        }
    }
}