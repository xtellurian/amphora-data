using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Models.Queries;
using Amphora.Api.Stores;
using Amphora.Common.Models;
using Amphora.Common.Models.Amphorae;
using Amphora.Tests.Helpers;
using Xunit;

namespace Amphora.Tests.Unit.Datastores
{
    public class QueryTests: UnitTestBase
    {
        [Fact]
        public async Task QueryAmphoraByLatLon()
        {
            var sut = new InMemoryEntityStore<AmphoraModel>(Mapper);
            
            var entity = EntityLibrary.GetAmphora(Guid.NewGuid().ToString(), nameof(QueryAmphoraByLatLon));

            entity = await sut.CreateAsync(entity);
            entity = await sut.ReadAsync<AmphoraExtendedModel>(entity.AmphoraId);
            var response = await sut.StartsWithQueryAsync<AmphoraExtendedModel>( "GeoHash" , entity.GeoHash.Substring(0, 3));
            Assert.Contains(response, r => string.Equals(r.Id, entity.Id));
        }
    }
}