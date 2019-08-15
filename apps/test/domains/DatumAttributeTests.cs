using System.Linq;
using Amphora.Common.Models.Domains.Dev;
using Xunit;

namespace Amphora.Tests.Unit
{
    public class DatumAttributeTests
    {
        [Fact]
        public void GetColumnsUsingDatumMember_DevDomain()
        {
            var devDomain = new DevDomain();
            var members = devDomain.GetDatumMembers();
            Assert.True(members.Count > 0);
            Assert.Contains(members, m => string.Equals(m.Name, "id"));
        }
    }
}