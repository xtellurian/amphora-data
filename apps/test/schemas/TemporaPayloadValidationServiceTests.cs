using System;
using Newtonsoft.Json.Linq;
using Amphora.Schemas.Library;
using Xunit;
using Amphora.Api.Contracts;
using Amphora.Common.Models;
using System.Threading.Tasks;

namespace Amphora.Tests.Unit
{
    public class TemporaPayloadValidationServiceTests
    {
        public TemporaPayloadValidationServiceTests(ITemporaPayloadValidationService sut, IOrgScopedEntityStore<Tempora> store)
        {
            this.sut = sut;
            this.store = store;
        }
        private const string randomJson = @"{'thing': 'value' }";
        private const string idValueJson = @"{'id': '1234', 'value': 1234 }";
        private const string idValueJsonWithExtras = @"{'id': '1234', 'value': 1234, 'thing': 'somevalue' }";
        private const string idValueJsonWithTime = @"{'id': '1234', 'value': 1234, 't': '2019-08-06T12:58:41.968966+10:00' }";
        private const string idValueJsonWithExtrasWithTime = @"{'id': '1234', 'value': 1234, 'thing': 'somevalue', 't': '2019-08-06T12:58:41.968966+10:00' }";
        private readonly ITemporaPayloadValidationService sut;
        private readonly IOrgScopedEntityStore<Tempora> store;

        [Theory]
        [InlineData( SchemaLibrary.IdValueSchema.LibraryId, idValueJson, true)]
        [InlineData( SchemaLibrary.IdValueSchema.LibraryId, idValueJsonWithTime, true)]
        [InlineData( SchemaLibrary.IdValueSchema.LibraryId, randomJson, false)]
        [InlineData( SchemaLibrary.IdValueSchema.LibraryId, idValueJsonWithExtras, false)]
        [InlineData( SchemaLibrary.IdValueSchema.LibraryId, idValueJsonWithExtrasWithTime, false)]
        public async Task IsValue_SuccessAndFail(string schemaId, string payloadString, bool expectedResult)
        {
            // arrange
            var tempora = await store.CreateAsync(new Tempora{
                Title = nameof(TemporaPayloadValidationServiceTests),
                Description = nameof(TemporaPayloadValidationServiceTests),
                Price = 2,
                SchemaId = schemaId
            });

            var payload = JObject.Parse(payloadString);

            // act
            var result = await sut.IsValidAsync(tempora, payload);

            // assert
            Assert.Equal(expectedResult, result);
        }
    }
}
