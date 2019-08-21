using System.Linq;
using Amphora.Common.Models.Domains.Dev;
using Xunit;

namespace Amphora.Tests.Unit
{
    public class DatumAttributeTests
    {
        [Fact]
        public void GetColumnsUsingDatumMember_DefaultDomain()
        {
            var defaultDomain = new DefaultDomain();
            var members = defaultDomain.GetDatumMembers();
            Assert.True(members.Count > 0);
            Assert.Contains(members, m => string.Equals(m.Name, "id"));
        }
    }
}