using System;
using Amphora.Common.Extensions;
using Amphora.Common.Models;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Permissions;
using Xunit;

namespace Amphora.Tests.Unit
{
    public class IdTests
    {
        // this test helps with backwards compat
        [Theory]
        [InlineData("Amphora", typeof(AmphoraModel))]
        [InlineData("Organisation", typeof(OrganisationModel))]
        public void IdShouldBeginWithAmphora(string prefix, Type t)
        {
            var id = System.Guid.NewGuid().ToString();
            var qId = id.AsQualifiedId(t);
            Assert.Equal($"{prefix}|{id}", qId);
            // do a second time to make sure it's idempotent
            var qId2 = qId.AsQualifiedId(t);
            Assert.Equal(qId, qId2);
        }

        [Fact]
        public void IdShouldBeginWithOnError()
        {
            string x = null;
            Assert.Throws<System.ArgumentException>(() => x.AsQualifiedId(typeof(AmphoraModel)));
            var y = Guid.NewGuid().ToString();
            Assert.Throws<System.ArgumentException>(() => y.AsQualifiedId(typeof(string)));
        }
    }
}
