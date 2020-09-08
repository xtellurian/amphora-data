using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Azure.TimeSeriesInsights.Models;
using Microsoft.Rest.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Xunit;

namespace Amphora.Tests.Unit.Casting
{
    public class TimeSeriesInsightsVariablesTest : UnitTestBase
    {
        // these were copied from the TimeSeriesInsights SDK
        private JsonSerializerSettings GetSerialisationSettings()
        {
            var serializationSettings = new JsonSerializerSettings();
            serializationSettings.Converters.Add(new PolymorphicSerializeJsonConverter<Variable>("kind"));
            return serializationSettings;
        }

        private JsonSerializerSettings GetDeserialisationSettings()
        {
            var deserializationSettings = new JsonSerializerSettings();
            deserializationSettings.Converters.Add(new PolymorphicDeserializeJsonConverter<Variable>("kind")); // does the order matter? YES!
            return deserializationSettings;
        }

        [Fact]
        public void NumericVariable_Serialised_DeserialisedBack()
        {
            var amphoraId = Guid.NewGuid().ToString();
            var propertyName = "prop";

            var query = new QueryRequest
            {
                GetSeries = new GetSeries(
                   new List<object> { amphoraId },
                   new DateTimeRange(DateTime.UtcNow.AddDays(-2), DateTime.UtcNow),
                   null, null,
                   new Dictionary<string, Variable>
                   {
                        {
                            propertyName, new NumericVariable(
                                value: new Tsx($"$event.{propertyName}"),
                                aggregation: new Tsx("avg($value)"))
                        }
                   })
            };

            // serialise
            var s = JsonConvert.SerializeObject(query, GetSerialisationSettings());
            // deserialise
            var ds = JsonConvert.DeserializeObject<QueryRequest>(s, GetDeserialisationSettings());

            ds.GetSeries.InlineVariables.Should().HaveCount(1);
            var variable = ds.GetSeries.InlineVariables[propertyName];
            variable.Should().BeOfType<NumericVariable>();
        }
    }
}