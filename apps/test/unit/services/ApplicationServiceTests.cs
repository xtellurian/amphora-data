using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.EntityFramework;
using Amphora.Api.Services.Applications;
using Amphora.Common.Models.Applications;
using Amphora.Common.Models.Organisations.Accounts;
using Amphora.Common.Models.Users;
using Amphora.Infrastructure.Database.Contexts;
using Amphora.Infrastructure.Stores.Applications;
using Amphora.Tests.Helpers;
using Amphora.Tests.Mocks;
using FluentAssertions;
using Xunit;

namespace Amphora.Tests.Unit.Services
{
    public class ApplicationServiceTests : UnitTestBase
    {
        [Fact]
        public async Task CreateApplicationFails_DefaultApplicationModelCtor()
        {
            // setup
            var appsContext = GetContext<ApplicationsContext>();
            var context = GetContext<AmphoraContext>();
            var principal = new TestPrincipal();
            var organisation = EntityLibrary.GetOrganisationModel();
            // only instituion plans can make apps
            organisation.Account.Plan.PlanType = Plan.PlanTypes.Institution;
            organisation.Id = "1234";
            var user = new ApplicationUserDataModel();
            user.OrganisationId = organisation.Id;
            user.Organisation = organisation;
            var userDataService = MockUser(principal, user);
            var store = new ApplicationModelEFStore(appsContext, CreateMockLogger<ApplicationModelEFStore>());
            var sut = new ApplicationService(store, userDataService.Object, CreateMockLogger<ApplicationService>());

            var app = new ApplicationModel();

            // act
            var res = await sut.CreateAsync(principal, app);
            res.Succeeded.Should().BeFalse("because we didn't set the properties in the application model");
            res.Message.Should().NotBeEmpty();
        }

        [Fact]
        public async Task CreateApplicationFails_OrgNotInstitutionPlan()
        {
            // setup
            var appsContext = GetContext<ApplicationsContext>();
            var context = GetContext<AmphoraContext>();
            var principal = new TestPrincipal();
            var organisation = EntityLibrary.GetOrganisationModel();
            // only instituion plans can make apps
            organisation.Account.Plan.PlanType = Plan.PlanTypes.Team;
            organisation.Id = "1234";
            var user = new ApplicationUserDataModel();
            user.OrganisationId = organisation.Id;
            user.Organisation = organisation;
            var userDataService = MockUser(principal, user);
            var store = new ApplicationModelEFStore(appsContext, CreateMockLogger<ApplicationModelEFStore>());
            var sut = new ApplicationService(store, userDataService.Object, CreateMockLogger<ApplicationService>());

            var app = new ApplicationModel
            {
                Name = "Hello World"
            };

            // act
            var res = await sut.CreateAsync(principal, app);
            res.Succeeded.Should().BeFalse("because we didn't set the properties in the application model");
            res.Message.Should().Be("Institution or Glaze plan is required to create applications");
        }

        [Fact]
        public async Task CanCreateApplication_ThenDelete_HappyPath()
        {
            // setup
            var appsContext = GetContext<ApplicationsContext>();
            var context = GetContext<AmphoraContext>();
            var principal = new TestPrincipal();
            var organisation = EntityLibrary.GetOrganisationModel();
            // only instituion plans can make apps
            organisation.Account.Plan.PlanType = Plan.PlanTypes.Institution;
            organisation.Id = "1234";
            var user = new ApplicationUserDataModel();
            user.OrganisationId = organisation.Id;
            user.Organisation = organisation;
            organisation.AddOrUpdateMembership(user, Common.Models.Organisations.Roles.Administrator);
            var userDataService = MockUser(principal, user);
            var store = new ApplicationModelEFStore(appsContext, CreateMockLogger<ApplicationModelEFStore>());
            var sut = new ApplicationService(store, userDataService.Object, CreateMockLogger<ApplicationService>());

            var app = new ApplicationModel
            {
                Name = "Hello World",
                Locations = new List<ApplicationLocationModel>
                {
                    new ApplicationLocationModel
                    {
                        Origin = "http://localhost:7000",
                        AllowedRedirectPaths = new List<string>
                        {
                            "/signign"
                        }
                    }
                }
            };

            // create
            var res = await sut.CreateAsync(principal, app);
            res.Succeeded.Should().BeTrue();
            res.Entity.Id.Should().NotBeNull();
            res.Entity.LastModified.Should().NotBeNull();
            res.Entity.CreatedDate.Should().NotBeNull();
            res.Entity.Name.Should().NotBeNull().And.Be("Hello World");
            res.Entity.Locations.Should().NotBeNull().And.HaveCount(1);

            // delete
            var deleteRes = await sut.DeleteAsync(principal, res.Entity.Id);
            deleteRes.Succeeded.Should().BeTrue();
        }

        [Fact]
        public async Task CanUpdateApplication()
        {
            // setup
            var appsContext = GetContext<ApplicationsContext>();
            var context = GetContext<AmphoraContext>();
            var principal = new TestPrincipal();
            var organisation = EntityLibrary.GetOrganisationModel();
            // only instituion plans can make apps
            organisation.Account.Plan.PlanType = Plan.PlanTypes.Institution;
            organisation.Id = "1234";
            var user = new ApplicationUserDataModel();
            user.OrganisationId = organisation.Id;
            user.Organisation = organisation;
            organisation.AddOrUpdateMembership(user, Common.Models.Organisations.Roles.Administrator);
            var userDataService = MockUser(principal, user);
            var store = new ApplicationModelEFStore(appsContext, CreateMockLogger<ApplicationModelEFStore>());
            var sut = new ApplicationService(store, userDataService.Object, CreateMockLogger<ApplicationService>());

            var app = new ApplicationModel
            {
                Name = "Hello World",
                Locations = new List<ApplicationLocationModel>
                {
                    new ApplicationLocationModel
                    {
                        Origin = "http://localhost:7000",
                        AllowedRedirectPaths = new List<string>
                        {
                            "/signign"
                        }
                    }
                }
            };

            // create
            var res = await sut.CreateAsync(principal, app);
            res.Succeeded.Should().BeTrue();

            // update
            var model = res.Entity;
            model.Name = "A new name";
            model.Locations = new List<ApplicationLocationModel>
            {
                new ApplicationLocationModel
                {
                    Origin = "http://localhost:44",
                    AllowedRedirectPaths = new List<string>
                    {
                        "/login"
                    }
                }
            };

            var updated = await sut.UpdateAsync(principal, model);
            updated.Succeeded.Should().BeTrue();
            updated.Entity.Name.Should().Be("A new name");
            updated.Entity.Locations.Should().HaveCount(1);
            updated.Entity.Locations.FirstOrDefault().Origin.Should().Be("http://localhost:44");

            // now delete
            var deleteRes = await sut.DeleteAsync(principal, res.Entity.Id);
            deleteRes.Succeeded.Should().BeTrue();
        }
    }
}