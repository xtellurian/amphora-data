using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Amphora.Api.Models.Dtos.Amphorae;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Integration.Amphorae
{
    [Collection(nameof(ApiFixtureCollection))]
    public class AmphoraFilesTests : WebAppIntegrationTestBase
    {
        public AmphoraFilesTests(WebApplicationFactory<Amphora.Api.Startup> factory) : base(factory)
        {
        }

        [Theory]
        [InlineData("/api/amphorae")]
        public async Task Post_UploadDownloadFilesAsAdmin_ThenDelete(string url)
        {
            // Arrange
            var client = await GetUserAsync();

            var amphora = Helpers.EntityLibrary.GetAmphoraDto(client.Organisation.Id);
            // create an amphora for us to work with
            var createResponse = await client.Http.PostAsync(url,
                new StringContent(JsonConvert.SerializeObject(amphora), Encoding.UTF8, "application/json"));
            await AssertHttpSuccess(createResponse);
            var createResponseContent = await createResponse.Content.ReadAsStringAsync();
            amphora = JsonConvert.DeserializeObject<DetailedAmphora>(createResponseContent);

            var generator = new Helpers.RandomGenerator(1024);
            var content = generator.GenerateBufferFromSeed(1024);
            var requestBody = new ByteArrayContent(content);
            var file = Guid.NewGuid().ToString();

            // Act
            var uploadResponse = await client.Http.PutAsync($"{url}/{amphora.Id}/files/{file}", requestBody);
            await AssertHttpSuccess(uploadResponse);

            var downloadResponse = await client.Http.GetAsync($"{url}/{amphora.Id}/files/{file}");
            await AssertHttpSuccess(downloadResponse);

            // Assert
            Assert.Equal(content, await downloadResponse.Content.ReadAsByteArrayAsync());

            // now delete the file
            var deleteRes = await client.Http.DeleteAsync($"{url}/{amphora.Id}/files/{file}");
            await AssertHttpSuccess(deleteRes);

            // check it's not there
            var downloadResponse_afterDelete = await client.Http.GetAsync($"{url}/{amphora.Id}/files/{file}");
            Assert.Equal(HttpStatusCode.NotFound, downloadResponse_afterDelete.StatusCode);

            // cleanup
            await DeleteAmphora(client.Http, amphora.Id);
        }

        [Theory]
        [InlineData("/api/amphorae")]
        public async Task Post_DownloadFiles_AsOtherUsers(string url)
        {
            // Arrange
            var client = await GetUserAsync(Users.Standard);

            var amphora = Helpers.EntityLibrary.GetAmphoraDto(client.Organisation.Id, nameof(Post_DownloadFiles_AsOtherUsers));
            // create an amphora for us to work with
            var createResponse = await client.Http.PostAsync(url,
                new StringContent(JsonConvert.SerializeObject(amphora), Encoding.UTF8, "application/json"));
            await AssertHttpSuccess(createResponse);
            var createResponseContent = await createResponse.Content.ReadAsStringAsync();
            amphora = JsonConvert.DeserializeObject<DetailedAmphora>(createResponseContent);

            var generator = new Helpers.RandomGenerator(1024);
            var content = generator.GenerateBufferFromSeed(1024);
            var requestBody = new ByteArrayContent(content);
            var file = Guid.NewGuid().ToString();

            client.Http.DefaultRequestHeaders.Add("Accept", "application/json");
            var uploadResponse = await client.Http.PutAsync($"{url}/{amphora.Id}/files/{file}", requestBody);
            await AssertHttpSuccess(uploadResponse);

            // Act and Assert
            // now let's download by someone in the same org - should work
            var sameOrgClient = await GetUserAsync(Users.StandardTwo);
            var downloadResponse = await sameOrgClient.Http.GetAsync($"{url}/{amphora.Id}/files/{file}");
            await AssertHttpSuccess(downloadResponse);
            Assert.Equal(content, await downloadResponse.Content.ReadAsByteArrayAsync());

            // other org user is denied access
            var other = await GetUserAsync(Users.Other);
            downloadResponse = await other.Http.GetAsync($"{url}/{amphora.Id}/files/{file}");
            Assert.Equal(HttpStatusCode.Forbidden, downloadResponse.StatusCode);

            // cleanup
            await DeleteAmphora(client.Http, amphora.Id);
            await DestroyAsync(client);
            await DestroyAsync(other);
        }

        [Theory]
        [InlineData("/api/amphorae")]
        public async Task Post_UploadFileToAmphora_MissingEntity(string url)
        {
            // Arrange
            var (client, user, org) = await NewOrgAuthenticatedClientAsync();
            var generator = new Helpers.RandomGenerator(1024);
            var content = generator.GenerateBufferFromSeed(1024);
            var requestBody = new ByteArrayContent(content);

            // Act
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            var uploadFileResponse = await client.PutAsync($"{url}/{Guid.NewGuid()}/files/{Guid.NewGuid()}", requestBody);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NotFound, uploadFileResponse.StatusCode);

            await DestroyOrganisationAsync(client, org);
            await DestroyUserAsync(client, user);
        }

        [Fact]
        public async Task CanSetMetadataForFiles()
        {
            // Arrange
            var url = "api/amphorae";
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();

            var amphora = Helpers.EntityLibrary.GetAmphoraDto(adminOrg.Id);
            // create an amphora for us to work with
            var createResponse = await adminClient.PostAsync(url,
                new StringContent(JsonConvert.SerializeObject(amphora), Encoding.UTF8, "application/json"));
            await AssertHttpSuccess(createResponse);
            var createResponseContent = await createResponse.Content.ReadAsStringAsync();
            amphora = JsonConvert.DeserializeObject<DetailedAmphora>(createResponseContent);
            // create a file for us to work with
            var generator = new Helpers.RandomGenerator(1024);
            var content = generator.GenerateBufferFromSeed(1024);
            var requestBody = new ByteArrayContent(content);
            var file = Guid.NewGuid().ToString();
            adminClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var uploadResponse = await adminClient.PutAsync($"{url}/{amphora.Id}/files/{file}", requestBody);
            await AssertHttpSuccess(uploadResponse);

            // Act
            var testMetadata = new Dictionary<string, string>()
            {
                { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() },
                { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() },
                { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() },
                { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() }
            };
            var createMetaRes = await adminClient.PostAsJsonAsync($"{url}/{amphora.Id}/files/{file}/meta", testMetadata);
            await AssertHttpSuccess(createMetaRes);
            // get the amphora again
            var readRes = await adminClient.GetAsync($"{url}/{amphora.Id}");
            await AssertHttpSuccess(readRes);
            amphora = JsonConvert.DeserializeObject<DetailedAmphora>(await readRes.Content.ReadAsStringAsync());
            Assert.NotNull(amphora.FileAttributes);
            Assert.Equal(testMetadata, amphora.FileAttributes[file].Attributes);
        }

        [Fact]
        public async Task UploadFileTwice_DuplicateError()
        {
            // Arrange
            var url = "api/amphorae";
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();

            var amphora = Helpers.EntityLibrary.GetAmphoraDto(adminOrg.Id);
            // create an amphora for us to work with
            var createResponse = await adminClient.PostAsync(url,
                new StringContent(JsonConvert.SerializeObject(amphora), Encoding.UTF8, "application/json"));
            await AssertHttpSuccess(createResponse);
            var createResponseContent = await createResponse.Content.ReadAsStringAsync();
            amphora = JsonConvert.DeserializeObject<DetailedAmphora>(createResponseContent);
            // create a file for us to work with
            var generator = new Helpers.RandomGenerator(1024);
            var content = generator.GenerateBufferFromSeed(1024);
            var requestBody = new ByteArrayContent(content);
            var file = Guid.NewGuid().ToString();
            // upload the first time
            var uploadResponse = await adminClient.PutAsync($"{url}/{amphora.Id}/files/{file}", requestBody);
            await AssertHttpSuccess(uploadResponse);
            // upload the second time
            var uploadResponse2 = await adminClient.PutAsync($"{url}/{amphora.Id}/files/{file}", requestBody);
            Assert.False(uploadResponse2.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.Conflict, uploadResponse2.StatusCode);
        }

        private async Task DeleteAmphora(HttpClient client, string id)
        {
            var deleteResponse = await client.DeleteAsync($"/api/amphorae/{id}");
            var response = await client.GetAsync($"api/amphorae/{id}");
            await AssertHttpSuccess(deleteResponse);
        }
    }
}