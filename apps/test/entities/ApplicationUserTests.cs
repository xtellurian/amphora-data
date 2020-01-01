using System.Linq;
using System.Threading.Tasks;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Users;
using Xunit;

namespace Amphora.Tests.Unit.Entities
{
    public class ApplicationUserTests: UnitTestBase
    {
        [Fact]
        public async Task CanPinAmphorae()
        {
            var context = base.GetContext();
            
            var user = new ApplicationUser();

            await context.Users.AddAsync(user);

            var amphora1 = new AmphoraModel();
            await context.Amphorae.AddAsync(amphora1);
            var amphora2 = new AmphoraModel();
            await context.Amphorae.AddAsync(amphora2);

            user.PinnedAmphorae.Amphora1 = amphora1;
            user.PinnedAmphorae.AmphoraId1 = amphora1.Id;
            user.PinnedAmphorae.Amphora2 = amphora2;
            user.PinnedAmphorae.AmphoraId2 = amphora2.Id;

            await context.SaveChangesAsync();

            user = context.Users.FirstOrDefault(); // reload user

            Assert.NotNull(user.PinnedAmphorae);
            Assert.NotNull(user.PinnedAmphorae.Amphora1);
            Assert.NotNull(user.PinnedAmphorae.AmphoraId1);
            Assert.NotNull(user.PinnedAmphorae.Amphora1.Id);
            Assert.Equal(amphora1.Id, user.PinnedAmphorae.Amphora1.Id);
        }
    }
}