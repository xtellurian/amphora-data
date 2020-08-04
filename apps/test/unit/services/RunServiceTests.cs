using System;
using System.Threading.Tasks;
using Amphora.Api.Services.Auth;
using Amphora.Api.Stores.EFCore;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Activities;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Users;
using Amphora.Common.Services.Activities;
using Amphora.Tests.Helpers;
using Amphora.Tests.Mocks;
using FluentAssertions;
using Xunit;

namespace Amphora.Tests.Unit.Services
{
    public class RunServiceTests : UnitTestBase
    {
        private MockDateTimeProvider dtProvider = new MockDateTimeProvider();

        [Fact]
        public async Task CanStartAndFinishARun_Successfully_AsNonOrgAdmin()
        {
            // setup
            var context = GetContext();
            var amphoraStore = new AmphoraeEFStore(context, CreateMockLogger<AmphoraeEFStore>());
            var activityStore = new ActivitiesEFStore(context, CreateMockLogger<ActivitiesEFStore>());
            var orgStore = new OrganisationsEFStore(context, CreateMockLogger<OrganisationsEFStore>());
            var creatorOrg = await orgStore.CreateAsync(EntityLibrary.GetOrganisationModel());
            var creatorPrincipal = new TestPrincipal();
            var creatorUserData = new ApplicationUserDataModel
            {
                Id = Guid.NewGuid().ToString(),
                OrganisationId = creatorOrg.Id,
                Organisation = creatorOrg
            };
            creatorOrg.Memberships.Add(new Membership(creatorUserData, Roles.User)); // not an admin
            creatorOrg = await orgStore.UpdateAsync(creatorOrg);
            var userDataSvc = MockUser(creatorPrincipal, creatorUserData);
            var permissionService = new PermissionService(orgStore, null, userDataSvc.Object, CreateMockLogger<PermissionService>());
            var sut = new ActivityRunService(activityStore, amphoraStore, userDataSvc.Object, permissionService, dtProvider, CreateMockLogger<ActivityRunService>());

            var activity = await activityStore.CreateAsync(new ActivityModel
            {
                Organisation = creatorOrg,
                OrganisationId = creatorOrg.Id
            });

            var createRunRes = await sut.StartRunAsync(creatorPrincipal, activity);
            Assert.True(createRunRes.Succeeded);
            Assert.NotNull(createRunRes.Entity);
            Assert.Contains(createRunRes.Entity, activity.Runs);

            // act
            var runRes = await sut.StartRunAsync(creatorPrincipal, activity);

            // assert
            runRes.Succeeded.Should().BeTrue("because the operation worked");
            runRes.Entity.Should().NotBeNull("because the service should return the entity");
            var run = runRes.Entity;
            run.Id.Should().NotBeNull("because the run needs an ID automatically set");
            run.StartTime.Should().Be(dtProvider.UtcNow, "because the start time is utc now");
            run.EndTime.Should().BeNull("because the run is still going");
            run.Success.Should().BeNull("because the run is still going");
            run.StartedByUserId.Should().Be(creatorUserData.Id, "because the run was created by this user");

            // fast forward in time
            dtProvider.GetNow = () => DateTime.Now;
            // now finish it
            var finishRes = await sut.FinishRunAsync(creatorPrincipal, activity, run, true);
            finishRes.Succeeded.Should().BeTrue("because we got the entity");
            run = finishRes.Entity;
            run.Success.Should().BeTrue("because the run completed successfully");
            run.EndTime.Should().Be(dtProvider.UtcNow, "because it completed now");
        }

        [Fact]
        public async Task CanReferenceAmphora_InARun_AsTheRunCreator()
        {
            // setup
            var context = GetContext();
            var amphoraStore = new AmphoraeEFStore(context, CreateMockLogger<AmphoraeEFStore>());
            var activityStore = new ActivitiesEFStore(context, CreateMockLogger<ActivitiesEFStore>());
            var orgStore = new OrganisationsEFStore(context, CreateMockLogger<OrganisationsEFStore>());
            var creatorOrg = await orgStore.CreateAsync(EntityLibrary.GetOrganisationModel());
            var creatorPrincipal = new TestPrincipal();
            var creatorUserData = new ApplicationUserDataModel
            {
                Id = Guid.NewGuid().ToString(),
                OrganisationId = creatorOrg.Id,
                Organisation = creatorOrg
            };
            creatorOrg.Memberships.Add(new Membership(creatorUserData, Roles.User)); // not an admin
            creatorOrg = await orgStore.UpdateAsync(creatorOrg);
            var userDataSvc = MockUser(creatorPrincipal, creatorUserData);
            var permissionService = new PermissionService(orgStore, null, userDataSvc.Object, CreateMockLogger<PermissionService>());
            var amphora = await amphoraStore.CreateAsync(EntityLibrary.GetAmphoraModel(creatorOrg));
            var sut = new ActivityRunService(activityStore, amphoraStore, userDataSvc.Object, permissionService, dtProvider, CreateMockLogger<ActivityRunService>());

            var activity = await activityStore.CreateAsync(new ActivityModel
            {
                Organisation = creatorOrg,
                OrganisationId = creatorOrg.Id
            });

            var createRunRes = await sut.StartRunAsync(creatorPrincipal, activity);
            Assert.True(createRunRes.Succeeded);

            // act
            var runRes = await sut.StartRunAsync(creatorPrincipal, activity);
            Assert.True(runRes.Succeeded);
            var run = runRes.Entity;
            // create the reference object
            var reference = new ActivityAmphoraReference(amphora, 1, 2, 3, 4);
            // reference an Amphora
            var referenceRes = await sut.ReferenceAmphoraAsync(creatorPrincipal, activity, run, reference);

            // assert
            Assert.True(referenceRes.Succeeded);
            Assert.NotNull(referenceRes.Entity);
            Assert.Contains(reference, run.AmphoraReferences);
            run.AmphoraReferences.Should().Contain(reference, "because we referenced that amphora");
            run.AmphoraReferences.Should().HaveCount(1, "because we references only 1 Amphora");
        }

        [Fact]
        public async Task ReferenceUnknownAmphora_ReturnsAnError_AsTheRunCreator()
        {
            // setup
            var context = GetContext();
            var amphoraStore = new AmphoraeEFStore(context, CreateMockLogger<AmphoraeEFStore>());
            var activityStore = new ActivitiesEFStore(context, CreateMockLogger<ActivitiesEFStore>());
            var orgStore = new OrganisationsEFStore(context, CreateMockLogger<OrganisationsEFStore>());
            var creatorOrg = await orgStore.CreateAsync(EntityLibrary.GetOrganisationModel());
            var creatorPrincipal = new TestPrincipal();
            var creatorUserData = new ApplicationUserDataModel
            {
                Id = Guid.NewGuid().ToString(),
                OrganisationId = creatorOrg.Id,
                Organisation = creatorOrg
            };
            creatorOrg.Memberships.Add(new Membership(creatorUserData, Roles.User)); // not an admin
            creatorOrg = await orgStore.UpdateAsync(creatorOrg);
            var userDataSvc = MockUser(creatorPrincipal, creatorUserData);
            var permissionService = new PermissionService(orgStore, null, userDataSvc.Object, CreateMockLogger<PermissionService>());
            var amphora = await amphoraStore.CreateAsync(EntityLibrary.GetAmphoraModel(creatorOrg));
            var sut = new ActivityRunService(activityStore, amphoraStore, userDataSvc.Object, permissionService, dtProvider, CreateMockLogger<ActivityRunService>());

            var activity = await activityStore.CreateAsync(new ActivityModel
            {
                Organisation = creatorOrg,
                OrganisationId = creatorOrg.Id
            });

            var createRunRes = await sut.StartRunAsync(creatorPrincipal, activity);
            Assert.True(createRunRes.Succeeded);

            // act
            var runRes = await sut.StartRunAsync(creatorPrincipal, activity);
            Assert.True(runRes.Succeeded);
            var run = runRes.Entity;
            // create the reference object
            var unknownReference = new ActivityAmphoraReference
            {
                AmphoraId = Guid.NewGuid().ToString()
            };
            // reference an Amphora
            var referenceRes = await sut.ReferenceAmphoraAsync(creatorPrincipal, activity, run, unknownReference);

            // assert
            Assert.True(referenceRes.Failed);
        }
    }
}