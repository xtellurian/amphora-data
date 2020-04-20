using Amphora.Common.Models.Events;
using Amphora.Tests.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Amphora.Tests.Unit.Entities
{
    public class EventDataTests : UnitTestBase
    {
        [Fact]
        public void AmphoraEventData_HasAmphoraId()
        {
            var org = EntityLibrary.GetOrganisationModel();
            var amphora = EntityLibrary.GetAmphoraModel(org);
            amphora.Id = System.Guid.NewGuid().ToString();

            var sut = new AmphoraCreatedEvent(amphora);

            var serialised = JsonConvert.SerializeObject(sut.Data);
            var jObject = JObject.Parse(serialised);
            var val = jObject.GetValue("AmphoraId").Value<string>();
            Assert.NotNull(val);
            Assert.Equal(amphora.Id, val);

            var friendlyName = jObject.GetValue("FriendlyName").Value<string>();
            Assert.NotNull(friendlyName);
            Assert.True(friendlyName.Length > 5);
        }
    }
}