using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using Amphora.Api;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Api.Models.Dtos.Amphorae.Signals;
using Amphora.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Azure.TimeSeriesInsights.Models;
using Microsoft.Rest.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Xunit;

namespace Amphora.Tests.Integration.TimeSeries
{
    [Collection(nameof(ApiFixtureCollection))]
    public class TimeSeriesQueryTests : WebAppIntegrationTestBase
    {
        public TimeSeriesQueryTests(WebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task CanQueryTSI_GetSeries()
        {
            // Arrange
            var persona = await GetPersonaAsync(Personas.Standard);

            // create an amphora
            var amphora = EntityLibrary.GetAmphoraDto(persona.Organisation.Id);
            var createResponse = await persona.Http.PostAsJsonAsync("api/amphorae", amphora);
            amphora = await AssertHttpSuccess<DetailedAmphora>(createResponse);

            // create a signal
            var generator = new RandomGenerator(1);
            var property = generator.RandomString(1).ToLower() + generator.RandomString(10) + "_" + generator.RandomString(2); // w/ underscore
            var signalDto = EntityLibrary.GetSignalDto(property);
            var response = await persona.Http.PostAsJsonAsync($"api/amphorae/{amphora.Id}/signals", signalDto);
            var sig = await AssertHttpSuccess<Signal>(response);

            // check we can make a request to teh TSI /query endpoint
            var query = new QueryRequest
            {
                GetSeries = new GetSeries(
                    new List<object> { amphora.Id },
                    new DateTimeRange(DateTime.UtcNow.AddDays(-2), DateTime.UtcNow),
                    null, null,
                    new Dictionary<string, Variable>
                    {
                        {
                            sig.Property, new NumericVariable(
                                value: new Tsx($"$event.{sig.Property}"),
                                aggregation: new Tsx("avg($value)"))
                        }
                    })
            };

            var formatter = new JsonMediaTypeFormatter();
            formatter.SerializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            formatter.SerializerSettings.Converters.Add(new PolymorphicSerializeJsonConverter<Variable>("kind"));

            var result = await persona.Http.PostAsync("/api/timeseries/query", query, formatter);
            await AssertHttpSuccess(result);
        }
    }
}
