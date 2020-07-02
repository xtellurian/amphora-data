using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Api.Models.Dtos.Amphorae.Files;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Integration.Amphorae
{
    [Collection(nameof(ApiFixtureCollection))]
    public class AmphoraFilesTests : WebAppIntegrationTestBase
    {
        private Helpers.RandomGenerator generator = new Helpers.RandomGenerator(1024);
        public AmphoraFilesTests(WebApplicationFactory<Amphora.Api.Startup> factory) : base(factory)
        {
        }

        [Theory]
        [InlineData("/api/amphorae")]
        public async Task Post_UploadDownloadFilesAsAdmin_ThenDelete(string url)
        {
            // Arrange
            var client = await GetPersonaAsync();

            var amphora = Helpers.EntityLibrary.GetAmphoraDto(client.Organisation.Id);
            // create an amphora for us to work with
            var createResponse = await client.Http.PostAsync(url,
                new StringContent(JsonConvert.SerializeObject(amphora), Encoding.UTF8, "application/json"));
            await AssertHttpSuccess(createResponse);
            var createResponseContent = await createResponse.Content.ReadAsStringAsync();
            amphora = JsonConvert.DeserializeObject<DetailedAmphora>(createResponseContent);

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
        public async Task Post_DownloadFiles_AsOtherUser(string url)
        {
            // Arrange
            var standard = await GetPersonaAsync(Personas.Standard);

            var amphora = Helpers.EntityLibrary.GetAmphoraDto(standard.Organisation.Id);
            // create an amphora for us to work with
            var createResponse = await standard.Http.PostAsync(url,
                new StringContent(JsonConvert.SerializeObject(amphora), Encoding.UTF8, "application/json"));
            await AssertHttpSuccess(createResponse);
            var createResponseContent = await createResponse.Content.ReadAsStringAsync();
            amphora = JsonConvert.DeserializeObject<DetailedAmphora>(createResponseContent);

            var content = generator.GenerateBufferFromSeed(1024);
            var requestBody = new ByteArrayContent(content);
            var file = Guid.NewGuid().ToString();

            standard.Http.DefaultRequestHeaders.Add("Accept", "application/json");
            var uploadResponse = await standard.Http.PutAsync($"{url}/{amphora.Id}/files/{file}", requestBody);
            await AssertHttpSuccess(uploadResponse);
            // uploader can download it
            var standardDownloadRes = await standard.Http.GetAsync($"{url}/{amphora.Id}/files/{file}");
            await AssertHttpSuccess(standardDownloadRes);

            // Act and Assert
            // now let's download by someone in the same org - should work
            var two = await GetPersonaAsync(Personas.StandardTwo);
            var downloadResponse = await two.Http.GetAsync($"{url}/{amphora.Id}/files/{file}");
            await AssertHttpSuccess(downloadResponse);
            Assert.Equal(content, await downloadResponse.Content.ReadAsByteArrayAsync());

            // other org user is denied access
            var other = await GetPersonaAsync(Personas.Other);
            downloadResponse = await other.Http.GetAsync($"{url}/{amphora.Id}/files/{file}");
            Assert.Equal(HttpStatusCode.Forbidden, downloadResponse.StatusCode);

            // cleanup
            await DeleteAmphora(standard.Http, amphora.Id);
        }

        [Theory]
        [InlineData("/api/amphorae")]
        public async Task Post_UploadFileToAmphora_MissingEntity(string url)
        {
            // Arrange
            var (client, user, org) = await NewOrgAuthenticatedClientAsync();
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
            var content = generator.GenerateBufferFromSeed(1024);
            var requestBody = new ByteArrayContent(content);
            var file = Guid.NewGuid().ToString();
            adminClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var uploadResponse = await adminClient.PutAsync($"{url}/{amphora.Id}/files/{file}", requestBody);
            await AssertHttpSuccess(uploadResponse);

            // Act
            var testMetadata = new Dictionary<string, string>()
            {
                { "abc", Guid.NewGuid().ToString() },
                { "def", Guid.NewGuid().ToString() }
            };
            var createMetaRes = await adminClient.PostAsJsonAsync($"{url}/{amphora.Id}/files/{file}/attributes", testMetadata);
            await AssertHttpSuccess(createMetaRes);
            // get the amphora again
            var readRes = await adminClient.GetAsync($"{url}/{amphora.Id}");
            await AssertHttpSuccess(readRes);
            amphora = JsonConvert.DeserializeObject<DetailedAmphora>(await readRes.Content.ReadAsStringAsync());

            // get attributes
            var attributesReq = await adminClient.GetAsync($"api/amphorae/{amphora.Id}/files/{file}/attributes");
            var attr = await AssertHttpSuccess<Dictionary<string, string>>(attributesReq);
            attr.Should().NotBeEmpty();
            attr.Should().HaveCount(testMetadata.Count);
            foreach (var kvp in testMetadata)
            {
                attr.Should().ContainKey(kvp.Key).And.ContainValue(kvp.Value);
            }

            await DeleteAmphora(adminClient, amphora.Id);
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

            await DeleteAmphora(adminClient, amphora.Id);
        }

        [Fact]
        public async Task ListFiles_CanQueryByAttribute()
        {
            var persona = await GetPersonaAsync(Personas.Standard);

            var amphora = Helpers.EntityLibrary.GetAmphoraDto(persona.Organisation.Id);
            // create an amphora for us to work with
            var createResponse = await persona.Http.PostAsJsonAsync("api/amphorae/", amphora);
            amphora = await AssertHttpSuccess<DetailedAmphora>(createResponse);

            // upload a file
            var content = generator.GenerateBufferFromSeed(1024);
            var requestBody = new ByteArrayContent(content);
            var file1 = Guid.NewGuid().ToString();
            var uploadResponse1 = await persona.Http.PutAsync($"api/amphorae/{amphora.Id}/files/{file1}", requestBody);
            var file2 = Guid.NewGuid().ToString();
            var uploadResponse2 = await persona.Http.PutAsync($"api/amphorae/{amphora.Id}/files/{file2}", requestBody);
            // add some attributes
            var testAttributes = new Dictionary<string, string>()
            {
                { "a", "foo" },
            };
            var createMetaRes = await persona.Http.PostAsJsonAsync($"api/amphorae/{amphora.Id}/files/{file1}/attributes", testAttributes);
            await AssertHttpSuccess(createMetaRes);
            // add another and push
            testAttributes.Add("b", "bar");
            var createMetaRes2 = await persona.Http.PostAsJsonAsync($"api/amphorae/{amphora.Id}/files/{file2}/attributes", testAttributes);
            await AssertHttpSuccess(createMetaRes2);

            var filter = new Dictionary<string, string>();
            var query = new FileQueryOptions
            {
                Attributes = filter
            };
            filter.Add("a", "foo");
            // now do a query with 1 attribute and correct attribute value
            var q = await persona.Http.PostAsJsonAsync($"api/amphorae/{amphora.Id}/files", query);
            var qFiles = await AssertHttpSuccess<List<string>>(q);
            qFiles.Should().HaveCount(2);
            filter.Clear();

            // now do a query with 1 attribute and INCORRECT attribute value
            filter.Add("a", "xxx");
            var qIncorrectValue = await persona.Http.PostAsJsonAsync($"api/amphorae/{amphora.Id}/files", query);
            var qIncorrectValueFiles = await AssertHttpSuccess<List<string>>(qIncorrectValue);
            qIncorrectValueFiles.Should().BeEmpty("because [a]=xxx never occurs in the attributes");
            filter.Clear();

            // now do a query with both attributes and All Attributes = true
            filter.Add("a", "foo");
            filter.Add("b", "bar");
            query.AllAttributes = true;
            var qBoth = await persona.Http.PostAsJsonAsync($"api/amphorae/{amphora.Id}/files", query);
            var qBothFiles = await AssertHttpSuccess<List<string>>(qBoth);
            qBothFiles.Should().HaveCount(1);
            filter.Clear();

            // now do a query with an extra attribute, but still should return 2 (all attributes is false)
            filter.Add("a", "foo");
            filter.Add("b", "bar");
            filter.Add("z", "zeta");
            query.AllAttributes = false;
            var qExtra = await persona.Http.PostAsJsonAsync($"api/amphorae/{amphora.Id}/files", query);
            var qExtraFiles = await AssertHttpSuccess<List<string>>(qExtra);
            qExtraFiles.Should().HaveCount(2);

            // now do a query with an extra attribute, but still should return 0 (all attributes is TRUE)
            query.AllAttributes = true;
            var qNone = await persona.Http.PostAsJsonAsync($"api/amphorae/{amphora.Id}/files", query);
            var qNoneFiles = await AssertHttpSuccess<List<string>>(qNone);
            qNoneFiles.Should().HaveCount(0);

            await DeleteAmphora(persona, amphora.Id);
        }

        [Fact]
        public async Task ListFiles_CanSortByLastModifiedOrAlpha()
        {
            var persona = await GetPersonaAsync(Personas.Standard);

            var amphora = Helpers.EntityLibrary.GetAmphoraDto(persona.Organisation.Id);
            // create an amphora for us to work with
            var createResponse = await persona.Http.PostAsJsonAsync("api/amphorae/", amphora);
            amphora = await AssertHttpSuccess<DetailedAmphora>(createResponse);

            // upload a file
            var content = generator.GenerateBufferFromSeed(1024);
            var requestBody = new ByteArrayContent(content);
            var alpha = "alpha";
            var beta = "beta";
            // beta get's uploaded first
            var uploadResponse2 = await persona.Http.PutAsync($"api/amphorae/{amphora.Id}/files/{beta}", requestBody);
            // wait a bit to ignore race conds
            await Task.Delay(1000);
            var uploadResponse1 = await persona.Http.PutAsync($"api/amphorae/{amphora.Id}/files/{alpha}", requestBody);

            // now do a query w/ orderBy
            var alphabeticalResponse = await persona.Http.GetAsync($"api/amphorae/{amphora.Id}/files?OrderBy=Alphabetical");
            var alphabeticalList = await AssertHttpSuccess<List<string>>(alphabeticalResponse);
            alphabeticalList.Should().HaveCount(2);
            alphabeticalList.Should().BeInAscendingOrder("because we sorted alphabetically");

            var lastModifiedResponse = await persona.Http.GetAsync($"api/amphorae/{amphora.Id}/files?OrderBy=LastModified");
            var lastModifiedList = await AssertHttpSuccess<List<string>>(lastModifiedResponse);
            lastModifiedList.Should().HaveCount(2);
            lastModifiedList.Should().NotBeInAscendingOrder("because we sorted by last modified");
        }

        [Fact]
        public async Task ListFiles_CanFilterByPrefix()
        {
            var persona = await GetPersonaAsync(Personas.Standard);

            var amphora = Helpers.EntityLibrary.GetAmphoraDto(persona.Organisation.Id);
            // create an amphora for us to work with
            var createResponse = await persona.Http.PostAsJsonAsync("api/amphorae/", amphora);
            amphora = await AssertHttpSuccess<DetailedAmphora>(createResponse);

            // upload some files
            await UploadFileWithRandomData(persona, amphora, "alpha");
            await UploadFileWithRandomData(persona, amphora, "beta");
            await UploadFileWithRandomData(persona, amphora, "delta");
            await UploadFileWithRandomData(persona, amphora, "epsilon");
            await UploadFileWithRandomData(persona, amphora, "zeta");

            // now do a query w/ orderBy
            var filtered = await persona.Http.GetAsync($"api/amphorae/{amphora.Id}/files?Prefix=al");
            var filteredList = await AssertHttpSuccess<List<string>>(filtered);
            filteredList.Should().HaveCount(1);
            filteredList.FirstOrDefault().Should().Be("alpha", "because the prefix starts with al");

            await DeleteAmphora(persona, amphora.Id);
        }

        [Fact]
        public async Task ListFiles_CanSkipAndTake()
        {
            var persona = await GetPersonaAsync(Personas.Standard);

            var amphora = Helpers.EntityLibrary.GetAmphoraDto(persona.Organisation.Id);
            // create an amphora for us to work with
            var createResponse = await persona.Http.PostAsJsonAsync("api/amphorae/", amphora);
            amphora = await AssertHttpSuccess<DetailedAmphora>(createResponse);

            // upload 5 files
            await UploadFileWithRandomData(persona, amphora, "alpha");
            await UploadFileWithRandomData(persona, amphora, "beta");
            await UploadFileWithRandomData(persona, amphora, "delta");
            await UploadFileWithRandomData(persona, amphora, "epsilon");
            await UploadFileWithRandomData(persona, amphora, "zeta");

            // now do a query w/ take=2
            var takeRes = await persona.Http.GetAsync($"api/amphorae/{amphora.Id}/files?Take=2");
            var takeList = await AssertHttpSuccess<List<string>>(takeRes);
            takeList.Should().HaveCount(2, "because we are only taking 2");

            // now do a query w/ skip=2 and take=2
            var skipped = await persona.Http.GetAsync($"api/amphorae/{amphora.Id}/files?Take=2&Skip=2");
            var skippedList = await AssertHttpSuccess<List<string>>(skipped);
            skippedList.Should().HaveCount(2, "because we are only taking 2").And.NotContain(takeList.FirstOrDefault());

            // now skip 5 and the list should return empty
            var allSkipped = await persona.Http.GetAsync($"api/amphorae/{amphora.Id}/files?Skip=5");
            var allSkippedList = await AssertHttpSuccess<List<string>>(allSkipped);
            allSkippedList.Should().BeEmpty("because we skipped all the files");

            await DeleteAmphora(persona, amphora.Id);
        }

        private async Task UploadFileWithRandomData(Helpers.Persona persona, DetailedAmphora amphora, string name)
        {
            var content = generator.GenerateBufferFromSeed(1024);
            var requestBody = new ByteArrayContent(content);
            var res = await persona.Http.PutAsync($"api/amphorae/{amphora.Id}/files/{name}", requestBody);
            await AssertHttpSuccess(res);
        }

        private async Task DeleteAmphora(HttpClient client, string id)
        {
            var deleteResponse = await client.DeleteAsync($"/api/amphorae/{id}");
            var response = await client.GetAsync($"api/amphorae/{id}");
            await AssertHttpSuccess(deleteResponse);
        }

        private async Task DeleteAmphora(Helpers.Persona persona, string id)
        {
            var deleteResponse = await persona.Http.DeleteAsync($"/api/amphorae/{id}");
            await AssertHttpSuccess(deleteResponse);
        }
    }
}