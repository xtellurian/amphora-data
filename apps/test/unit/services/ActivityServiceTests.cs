using System;
using System.Collections.Generic;
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
    public class ActivityServiceTests : UnitTestBase
    {
        private IDateTimeProvider dtProvider = new MockDateTimeProvider();

        [Fact]
        public async Task CanReadActivity_AsOrgUser()
        {
            // setup
            var context = GetContext();
            var activityStore = new ActivitiesEFStore(context, CreateMockLogger<ActivitiesEFStore>());
            var orgStore = new OrganisationsEFStore(context, CreateMockLogger<OrganisationsEFStore>());
            var org = await orgStore.CreateAsync(EntityLibrary.GetOrganisationModel());
            var principal = new TestPrincipal();
            var creatorUserData = new ApplicationUserDataModel
            {
                Id = Guid.NewGuid().ToString(),
                OrganisationId = org.Id,
                Organisation = org
            };
            org.Memberships.Add(new Membership(creatorUserData, Roles.User)); // an admin
            org = await orgStore.UpdateAsync(org);
            var userDataSvc = MockUser(principal, creatorUserData);
            var permissionService = new PermissionService(orgStore, null, userDataSvc.Object, CreateMockLogger<PermissionService>());
            var activity = NewActivityModel(org);

            activity = await activityStore.CreateAsync(activity);
            activity.Id.Should().NotBeNull("because the activity should have an id");
            var sut = new ActivityService(activityStore, userDataSvc.Object, permissionService, CreateMockLogger<ActivityService>());
            var readRes = await sut.ReadAsync(principal, activity.Id);
            readRes.Succeeded.Should().BeTrue("because we can read the entity");
            readRes.Entity.Should().NotBeNull("because the entity was returned");
            readRes.Entity.Should().Be(activity, "because we retrieved that entity");
        }

        [Fact]
        public async Task CanCreateActivity_AsNonOrgAdmin()
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
            var sut = new ActivityService(activityStore, userDataSvc.Object, permissionService, CreateMockLogger<ActivityService>());

            // act
            var model = new ActivityModel();
            var createRes = await sut.CreateAsync(creatorPrincipal, model);

            // assert
            Assert.True(createRes.Succeeded);
            Assert.NotNull(createRes.Entity);
            Assert.NotNull(createRes.Entity.Id);
            Assert.Empty(createRes.Entity.Runs);
        }

        [Fact]
        public async Task CanDeleteAnActivity_WithRuns_AsOrgAdmin()
        {
            // setup
            var context = GetContext();
            var activityStore = new ActivitiesEFStore(context, CreateMockLogger<ActivitiesEFStore>());
            var orgStore = new OrganisationsEFStore(context, CreateMockLogger<OrganisationsEFStore>());
            var org = await orgStore.CreateAsync(EntityLibrary.GetOrganisationModel());
            var adminPrincipal = new TestPrincipal();
            var creatorUserData = new ApplicationUserDataModel
            {
                Id = Guid.NewGuid().ToString(),
                OrganisationId = org.Id,
                Organisation = org
            };
            org.Memberships.Add(new Membership(creatorUserData, Roles.Administrator)); // an admin
            org = await orgStore.UpdateAsync(org);
            var userDataSvc = MockUser(adminPrincipal, creatorUserData);
            var permissionService = new PermissionService(orgStore, null, userDataSvc.Object, CreateMockLogger<PermissionService>());
            var model = NewActivityModel(org);

            model = await activityStore.CreateAsync(model);
            // delete a run
            var sut = new ActivityService(activityStore, userDataSvc.Object, permissionService, CreateMockLogger<ActivityService>());
            var deleteRes = await sut.DeleteAsync(adminPrincipal, model);

            // assert
            Assert.True(deleteRes.Succeeded);
            Assert.Null(deleteRes.Entity);
        }

        private ActivityModel NewActivityModel(OrganisationModel org)
        {
            return new ActivityModel
            {
                OrganisationId = org.Id,
                Organisation = org,
                Runs = new List<ActivityRun>
                {
                    new ActivityRun
                    {
                        StartTime = dtProvider.UtcNow
                    }
                }
            };
        }
    }
}