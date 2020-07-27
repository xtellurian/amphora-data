using Amphora.Api.Extensions;
using Amphora.Api.Models.Dtos.Search;
using FluentAssertions;
using Xunit;

namespace Amphora.Tests.Unit.Mapping
{
    public class SearchParameterMap
    {
        [Fact]
        public void DefaultQuery_Amphora_CanBeMapped()
        {
            var amphoraQuery = new AmphoraSearchQueryOptions();
            var searchParameters = amphoraQuery.ToSearchParameters();

            searchParameters.Top.Should().NotBeNull();
            searchParameters.Skip.Should().NotBeNull();
        }

        [Fact]
        public void DefaultQuery_DataRequests_CanBeMapped()
        {
            var drQuery = new DataRequestSearchQueryOptions();
            var searchParameters = drQuery.ToSearchParameters();

            searchParameters.Top.Should().NotBeNull();
            searchParameters.Skip.Should().NotBeNull();
        }
    }
}