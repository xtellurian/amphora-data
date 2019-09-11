using System;
using Amphora.Common.Extensions;
using Amphora.Common.Models;
using Amphora.Common.Models.Organisations;
using Xunit;

namespace Amphora.Tests.Unit
{
    public class IdTests
    {
        [Theory]
        [InlineData("AmphoraModel", typeof(AmphoraModel))]
        [InlineData("OrganisationModel", typeof(OrganisationModel))]
        [InlineData("PermissionModel", typeof(PermissionModel))]
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
            Assert.Throws<System.ArgumentException>(() => x.AsQualifiedId(typeof(Amphora.Common.Models.AmphoraModel)));
            var y = Guid.NewGuid().ToString();
            Assert.Throws<System.ArgumentException>(() => y.AsQualifiedId(typeof(string)));
        }
    }
}
