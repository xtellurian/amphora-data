using Amphora.Api.Models.Feeds;
using Amphora.Tests.Helpers;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Unit.Entities
{
    public class SerialiseFeedTests : UnitTestBase
    {
        [Fact]
        public void AmphoraCreatedPost_CanSerialise()
        {
            var org = EntityLibrary.GetOrganisationModel();
            var amphora = EntityLibrary.GetAmphoraModel(org);
            var post = new AmphoraCreatedPost(amphora);

            var serialised = JsonConvert.SerializeObject(post);
            // bit of a weird, do-nothing test eh?
        }
    }
}