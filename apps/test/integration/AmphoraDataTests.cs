using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Amphora.Common.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Integration
{
    public class AmphoraDataTests : IntegrationTestBase, IClassFixture<WebApplicationFactory<Amphora.Api.Startup>>
    {

        public AmphoraDataTests(WebApplicationFactory<Amphora.Api.Startup> factory) : base(factory)
        {
        }

        [Theory]
        [InlineData("/api/amphorae")]
        public async Task Post_UploadDownloadFiles_AsAdmin(string url)
        {
            // Arrange
            var (adminClient, adminUser, adminOrg) = await GetAuthenticatedClientAsync(RoleAssignment.Roles.Administrator);

            var amphora = Helpers.EntityLibrary.GetAmphora(adminOrg.OrganisationId);
            // create an amphora for us to work with
            var createResponse = await adminClient.PostAsync(url,
                new StringContent(JsonConvert.SerializeObject(amphora), Encoding.UTF8, "application/json")
                );
            createResponse.EnsureSuccessStatusCode();
            var createResponseContent = await createResponse.Content.ReadAsStringAsync();
            amphora = JsonConvert.DeserializeObject<Amphora.Common.Models.Amphora>(createResponseContent);

            var generator = new Helpers.RandomBufferGenerator(1024);
            var content = generator.GenerateBufferFromSeed(1024);
            var requestBody = new ByteArrayContent(content);
            var file = Guid.NewGuid().ToString();

            // Act
            adminClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var uploadResponse = await adminClient.PutAsync($"{url}/{amphora.Id}/files/{file}", requestBody);
            uploadResponse.EnsureSuccessStatusCode();

            var downloadResponse = await adminClient.GetAsync($"{url}/{amphora.Id}/files/{file}");
            downloadResponse.EnsureSuccessStatusCode();

            // Assert
            Assert.Equal(content, await downloadResponse.Content.ReadAsByteArrayAsync());

            // cleanup
            await DeleteAmphora(adminClient, amphora.Id);
            await DestroyUserAsync(adminClient);
            await DestroyOrganisationAsync(adminClient);
        }

        [Theory]
        [InlineData("/api/amphorae")]
        public async Task Post_DownloadFiles_AsOtherUsers(string url)
        {
            // Arrange
            var (adminClient, adminUser, adminOrg) = await GetAuthenticatedClientAsync(RoleAssignment.Roles.Administrator);

            var amphora = Helpers.EntityLibrary.GetAmphora(adminOrg.OrganisationId);
            // create an amphora for us to work with
            var createResponse = await adminClient.PostAsync(url,
                new StringContent(JsonConvert.SerializeObject(amphora), Encoding.UTF8, "application/json")
                );
            createResponse.EnsureSuccessStatusCode();
            var createResponseContent = await createResponse.Content.ReadAsStringAsync();
            amphora = JsonConvert.DeserializeObject<Amphora.Common.Models.Amphora>(createResponseContent);

            var generator = new Helpers.RandomBufferGenerator(1024);
            var content = generator.GenerateBufferFromSeed(1024);
            var requestBody = new ByteArrayContent(content);
            var file = Guid.NewGuid().ToString();

            adminClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var uploadResponse = await adminClient.PutAsync($"{url}/{amphora.Id}/files/{file}", requestBody);
            uploadResponse.EnsureSuccessStatusCode();

            // Act and Assert
            // now let's download by someone in the same org - should work
            var (sameOrgClient, sameOrgUser, sameOrgOrg) = await GetAuthenticatedClientAsync(RoleAssignment.Roles.User, adminOrg);
            var downloadResponse = await sameOrgClient.GetAsync($"{url}/{amphora.Id}/files/{file}");
            downloadResponse.EnsureSuccessStatusCode();
            Assert.Equal(content, await downloadResponse.Content.ReadAsByteArrayAsync());

            // other org user is denied access
            var (otherOrgClient, otherOrgUser, otherOrg) = await GetAuthenticatedClientAsync(RoleAssignment.Roles.User);
            downloadResponse = await otherOrgClient.GetAsync($"{url}/{amphora.Id}/files/{file}");
            Assert.Equal(HttpStatusCode.Forbidden,  downloadResponse.StatusCode);

            // cleanup
            await DeleteAmphora(adminClient, amphora.Id);
            await DestroyUserAsync(adminClient);
            await DestroyUserAsync(sameOrgClient);
            await DestroyOrganisationAsync(adminClient);

            await DestroyUserAsync(otherOrgClient);
            await DestroyOrganisationAsync(otherOrgClient);
        }

        [Theory]
        [InlineData("/api/amphorae")]
        public async Task Post_UploadToAmphora_MissingEntity(string url)
        {
            // Arrange
            var (client, user, org) = await GetAuthenticatedClientAsync();
            var generator = new Helpers.RandomBufferGenerator(1024);
            var content = generator.GenerateBufferFromSeed(1024);
            var requestBody = new ByteArrayContent(content);

            // Act
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            var fillResponse = await client.PutAsync($"{url}/{Guid.NewGuid()}/files/{Guid.NewGuid()}", requestBody);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NotFound, fillResponse.StatusCode);

            await DestroyUserAsync(client);
            await DestroyOrganisationAsync(client);
        }

        private async Task DeleteAmphora(HttpClient client, string id)
        {
            var deleteResponse = await client.DeleteAsync($"/api/amphorae/{id}");
            var response = await client.GetAsync($"api/amphorae/{id}");
            deleteResponse.EnsureSuccessStatusCode();
        }
    }
}