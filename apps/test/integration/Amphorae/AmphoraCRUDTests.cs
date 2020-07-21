using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Api.Models.Dtos.Terms;
using Amphora.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Integration.Amphorae
{
    [Collection(nameof(ApiFixtureCollection))]
    public class AmphoraCRUDTests : WebAppIntegrationTestBase
    {
        public AmphoraCRUDTests(WebApplicationFactory<Amphora.Api.Startup> factory) : base(factory) { }

        [Fact]
        public async Task Post_CreatesAmphora_AsAdmin()
        {
            var url = "/api/amphorae";
            // Arrange
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();
            var a = Helpers.EntityLibrary.GetAmphoraDto(adminOrg.Id, nameof(Post_CreatesAmphora_AsAdmin));
            var s = JsonConvert.SerializeObject(a);
            var requestBody = new StringContent(s, Encoding.UTF8, "application/json");

            // Act
            adminClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var response = await adminClient.PostAsync(url, requestBody);

            // Assert
            await AssertHttpSuccess(response);
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseBody);
            var b = JsonConvert.DeserializeObject<DetailedAmphora>(responseBody);
            Assert.NotNull(b.Id);
            Assert.Equal(a.Description, b.Description);
            Assert.Equal(a.Price, b.Price);
            Assert.Equal(a.Name, b.Name);
            Assert.Equal(a.Labels, b.Labels);

            // check it's in the list
            var listRes = await adminClient.GetAsync("/api/amphorae?accessType=created&scope=self");
            await AssertHttpSuccess(listRes);
            var listContent = await listRes.Content.ReadAsStringAsync();
            var selfCreatedAmphora = JsonConvert.DeserializeObject<List<DetailedAmphora>>(listContent);
            Assert.NotNull(selfCreatedAmphora);
            Assert.NotEmpty(selfCreatedAmphora);
            Assert.Single(selfCreatedAmphora);

            await DestroyAmphoraAsync(adminClient, b.Id);
            await DestroyOrganisationAsync(adminClient, adminOrg);
            await DestroyUserAsync(adminClient, adminUser);
        }

        [Fact]
        public async Task CreateAmphora_With10Labels()
        {
            var persona = await GetPersonaAsync();
            var amphora = Helpers.EntityLibrary.GetAmphoraDto(persona.Organisation.Id);
            var generator = new Helpers.RandomGenerator();
            var labelsList = new List<string>();
            for (var i = 0; i < 10; i++)
            {
                labelsList.Add(generator.RandomString(5));
            }

            var labels = string.Join(',', labelsList);
            amphora.Labels = labels;
            // Act
            var createResponse = await persona.Http.PostAsJsonAsync("api/amphorae", amphora);

            // Assert
            amphora = await AssertHttpSuccess<DetailedAmphora>(createResponse);

            Assert.Equal(amphora.Labels, labels);
        }

        [Fact]
        public async Task CanListAmphora_InVariousSituations()
        {
            // setcup the scipes up here
            var self = "scope=self";
            var org = "scope=organisation";
            var created = "accessType=created";
            var purchased = "accessType=purchased";

            // we need 2 people in org and 1 outside of org.
            // one of them needs to purchase something
            // and the other needs to do the get list
            var p1 = await GetPersonaAsync(Personas.Standard);
            var p2 = await GetPersonaAsync(Personas.StandardTwo);
            var other = await GetPersonaAsync(Personas.Other);

            // create the 3 amphora
            var amphora1 = EntityLibrary.GetAmphoraDto(p1.Organisation.Id);
            var createRes1 = await p1.Http.PostAsJsonAsync("api/amphorae", amphora1);
            amphora1 = await AssertHttpSuccess<DetailedAmphora>(createRes1);

            var amphora2 = EntityLibrary.GetAmphoraDto(p2.Organisation.Id);
            var createRes2 = await p2.Http.PostAsJsonAsync("api/amphorae", amphora2);
            amphora2 = await AssertHttpSuccess<DetailedAmphora>(createRes2);

            var otherAmphora = EntityLibrary.GetAmphoraDto(other.Organisation.Id);
            var createOtherRes = await other.Http.PostAsJsonAsync("api/amphorae", otherAmphora);
            otherAmphora = await AssertHttpSuccess<DetailedAmphora>(createOtherRes);

            // now get p2 to purchase the other's amphora
            var purchaseRes = await p2.Http.PostAsJsonAsync($"api/Amphorae/{otherAmphora.Id}/Purchases", new { });
            await AssertHttpSuccess(purchaseRes);

            // now do some listing and ensure they exist
            // p1s created amphora
            var res1 = await p1.Http.GetAsync($"api/amphorae?{self}&{created}");
            var list1 = await AssertHttpSuccess<List<DetailedAmphora>>(res1);
            list1.Should().Contain(_ => _.Id == amphora1.Id, "because p1 should see their own amphora");
            list1.Should().NotContain(_ => _.Id == amphora2.Id, "because p1 did not create amphora2");
            // p1 should be able to see p2's amphora
            var res2 = await p1.Http.GetAsync($"api/amphorae?{org}&{created}");
            var list2 = await AssertHttpSuccess<List<DetailedAmphora>>(res2);
            list2.Should().Contain(_ => _.Id == amphora1.Id, "because p1 should still see the amphora they created");
            list2.Should().Contain(_ => _.Id == amphora2.Id, "because p1 should be able to see p2s amphora");
            // p1 should be able to see p2's purchased amphora
            var res3 = await p1.Http.GetAsync($"api/amphorae?{org}&{purchased}");
            var list3 = await AssertHttpSuccess<List<DetailedAmphora>>(res3);
            list3.Should().Contain(_ => _.Id == otherAmphora.Id, "because p1 should be able to see p2s purchased amphora");
            list3.Should()
                .NotContain(_ => _.Id == amphora1.Id, "because created amphora 1 shouldn't show")
                .And
                .NotContain(_ => _.Id == amphora2.Id, "because created amphora 2 shouldn't show");
            // p2 should see their own purchase
            var res4 = await p2.Http.GetAsync($"api/amphorae?{self}&{purchased}");
            var list4 = await AssertHttpSuccess<List<DetailedAmphora>>(res4);
            list4.Should().Contain(_ => _.Id == otherAmphora.Id, "because p2 purchased the other amphora");
            list4.Should()
                .NotContain(_ => _.Id == amphora1.Id, "because created amphora 1 shouldn't show")
                .And
                .NotContain(_ => _.Id == amphora2.Id, "because created amphora 2 shouldn't show");
        }

        [Fact]
        public async Task Get_ReadsAmphora_AsAdmin()
        {
            var url = "/api/amphorae";
            // Arrange
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();

            var a = Helpers.EntityLibrary.GetAmphoraDto(adminOrg.Id, nameof(Get_ReadsAmphora_AsAdmin));
            var requestBody = new StringContent(JsonConvert.SerializeObject(a), Encoding.UTF8, "application/json");
            adminClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var createResponse = await adminClient.PostAsync(url, requestBody);
            await AssertHttpSuccess(createResponse);
            var b = JsonConvert.DeserializeObject<DetailedAmphora>(await createResponse.Content.ReadAsStringAsync());

            // Act
            adminClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var response = await adminClient.GetAsync($"{url}/{b.Id}");

            // Assert
            await AssertHttpSuccess(response);
            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            Assert.NotNull(responseBody);
            var c = JsonConvert.DeserializeObject<DetailedAmphora>(responseBody);
            Assert.Equal(b.Id, c.Id);
            Assert.Equal(b.Description, c.Description);
            Assert.Equal(b.Price, c.Price);
            Assert.Equal(b.Name, c.Name);

            // cleanup
            await DestroyAmphoraAsync(adminClient, b.Id);
            await DestroyOrganisationAsync(adminClient, adminOrg);
            await DestroyUserAsync(adminClient, adminUser);
        }

        [Fact]
        public async Task CanPaginate_MyAmphora()
        {
            var persona = await GetPersonaAsync(Personas.AmphoraAdmin);
            var total = 10;
            // create some amphora
            var allAmphoraCreated = new List<DetailedAmphora>();
            for (var n = 0; n < total; n++)
            {
                var amphora = EntityLibrary.GetAmphoraDto(persona.Organisation.Id);
                var createRes = await persona.Http.PostAsJsonAsync("api/amphorae", amphora);
                amphora = await AssertHttpSuccess<DetailedAmphora>(createRes);
                allAmphoraCreated.Add(amphora);
            }

            var res1 = await persona.Http.GetAsync($"api/amphorae");
            var list1 = await AssertHttpSuccess<List<DetailedAmphora>>(res1);
            list1.Should().HaveCountGreaterOrEqualTo(total);

            var res2 = await persona.Http.GetAsync($"api/amphorae?Take=2");
            var list2 = await AssertHttpSuccess<List<DetailedAmphora>>(res2);
            list2.Should().HaveCount(2);

            var res3 = await persona.Http.GetAsync($"api/amphorae?Skip=3&Take=2");
            var list3 = await AssertHttpSuccess<List<DetailedAmphora>>(res3);
            list3.Should().HaveCount(2);
            foreach (var a in list3)
            {
                list2.Should().NotContain(_ => _.Id == a.Id, "because the lists should be distinct");
            }
        }

        [Fact]
        public async Task Get_PublicAmphora_MetadataIsAvailable()
        {
            var url = "/api/amphorae";
            // Arrange
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();

            var dto = Helpers.EntityLibrary.GetAmphoraDto(adminOrg.Id);
            var requestBody = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            adminClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var createResponse = await adminClient.PostAsync(url, requestBody);
            var c = await createResponse.Content.ReadAsStringAsync();
            dto = JsonConvert.DeserializeObject<DetailedAmphora>(c);
            await AssertHttpSuccess(createResponse);
            // Act
            var (client, user, org) = await NewOrgAuthenticatedClientAsync();
            var response = await client.GetAsync($"{url}/{dto.Id}");
            var responseContent = await response.Content.ReadAsStringAsync();
            // Assert
            await AssertHttpSuccess(response);
            var b = JsonConvert.DeserializeObject<DetailedAmphora>(responseContent);
            Assert.NotNull(b);
            Assert.Equal(dto.Id, b.Id);

            await DestroyAmphoraAsync(adminClient, dto.Id);
            await DestroyOrganisationAsync(adminClient, adminOrg);
            await DestroyUserAsync(adminClient, adminUser);

            await DestroyOrganisationAsync(client, org);
            await DestroyUserAsync(client, user);
        }

        [Fact]
        public async Task PurchaseAmphora_DetailsRemainSame()
        {
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();
            var (otherClient, otherUser, otherOrg) = await NewOrgAuthenticatedClientAsync();

            var dto = Helpers.EntityLibrary.GetAmphoraDto(adminOrg.Id, nameof(Get_ReadsAmphora_AsAdmin));
            var requestBody = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            adminClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var createResponse = await adminClient.PostAsync("api/amphorae", requestBody);
            await AssertHttpSuccess(createResponse);
            dto = JsonConvert.DeserializeObject<DetailedAmphora>(await createResponse.Content.ReadAsStringAsync());

            var purchaseResponse = await otherClient.PostAsync($"api/Amphorae/{dto.Id}/Purchases", null);
            await AssertHttpSuccess(purchaseResponse);

            var readResponse = await adminClient.GetAsync($"api/amphorae/{dto.Id}");
            var content = await readResponse.Content.ReadAsStringAsync();
            await AssertHttpSuccess(readResponse);

            var b = JsonConvert.DeserializeObject<DetailedAmphora>(content);
            Assert.Equal(dto.Description, b.Description);
            Assert.True(b.Lat.HasValue);
            Assert.True(b.Lon.HasValue);
            Assert.Equal(dto.Lat, b.Lat);
            Assert.Equal(dto.Lon, b.Lon);

            await DestroyAmphoraAsync(adminClient, dto.Id);
            await DestroyOrganisationAsync(adminClient, adminOrg);
            await DestroyUserAsync(adminClient, adminUser);
        }

        [Fact]
        public async Task Amphora_WithTermsOfUse_MustAcceptToPurchase()
        {
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();
            var (otherClient, otherUser, otherOrg) = await NewOrgAuthenticatedClientAsync();

            // create some terms of use
            var tou = new TermsOfUse
            {
                Contents = "Some TOU contents",
                Name = System.Guid.NewGuid().ToString()
            };

            var touRes = await adminClient.PostAsJsonAsync($"/api/TermsOfUse", tou);
            await AssertHttpSuccess(touRes);
            tou = JsonConvert.DeserializeObject<TermsOfUse>(await touRes.Content.ReadAsStringAsync());
            Assert.NotNull(tou);
            Assert.NotNull(tou.Id);

            var dto = Helpers.EntityLibrary.GetAmphoraDto(adminOrg.Id);
            dto.TermsOfUseId = tou.Id;
            var requestBody = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            adminClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var createResponse = await adminClient.PostAsync("api/amphorae", requestBody);
            await AssertHttpSuccess(createResponse);
            dto = JsonConvert.DeserializeObject<DetailedAmphora>(await createResponse.Content.ReadAsStringAsync());

            // purchase it, should fail with need to accept TOU
            var purchaseResponse = await otherClient.PostAsync($"api/Amphorae/{dto.Id}/Purchases", null);
            Assert.False(purchaseResponse.IsSuccessStatusCode);

            // Accept the terms
            var acceptResponse = await otherClient.PostAsync($"api/TermsOfUse/{dto.TermsOfUseId}/Accepts", null);
            await AssertHttpSuccess(acceptResponse);

            // purchase it successfully
            purchaseResponse = await otherClient.PostAsync($"api/Amphorae/{dto.Id}/Purchases", null);
            await AssertHttpSuccess(purchaseResponse);

            var readResponse = await adminClient.GetAsync($"api/amphorae/{dto.Id}");
            var content = await readResponse.Content.ReadAsStringAsync();
            await AssertHttpSuccess(readResponse);

            var b = JsonConvert.DeserializeObject<DetailedAmphora>(content);
            Assert.Equal(dto.Description, b.Description);
            Assert.True(b.Lat.HasValue);
            Assert.True(b.Lon.HasValue);
            Assert.Equal(dto.Lat, b.Lat);
            Assert.Equal(dto.Lon, b.Lon);

            await DestroyAmphoraAsync(adminClient, dto.Id);
            await DestroyOrganisationAsync(otherClient, otherOrg);
            await DestroyUserAsync(otherClient, otherUser);
            await DestroyOrganisationAsync(adminClient, adminOrg);
            await DestroyUserAsync(adminClient, adminUser);
        }

        [Fact]
        public async Task CreateAmphora_MissingTermsOfUse_ShouldError()
        {
            var url = "/api/amphorae";
            // Arrange
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();
            var a = Helpers.EntityLibrary.GetAmphoraDto(adminOrg.Id);
            a.TermsOfUseId = System.Guid.NewGuid().ToString();
            var s = JsonConvert.SerializeObject(a);
            var requestBody = new StringContent(s, Encoding.UTF8, "application/json");

            // Act
            adminClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var response = await adminClient.PostAsync(url, requestBody);

            // Assert
            Assert.False(response.IsSuccessStatusCode);

            await DestroyOrganisationAsync(adminClient, adminOrg);
            await DestroyUserAsync(adminClient, adminUser);
        }

        [Fact]
        public async Task UpdateAmphora_Success()
        {
            var url = "/api/amphorae";
            // Arrange
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();
            var a = Helpers.EntityLibrary.GetAmphoraDto(adminOrg.Id, nameof(Post_CreatesAmphora_AsAdmin));
            var s = JsonConvert.SerializeObject(a);
            var requestBody = new StringContent(s, Encoding.UTF8, "application/json");

            // Act
            adminClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var response = await adminClient.PostAsync(url, requestBody);

            await AssertHttpSuccess(response);
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseBody);
            a = JsonConvert.DeserializeObject<DetailedAmphora>(responseBody);
            Assert.NotNull(a.Id);

            // now update
            var newName = System.Guid.NewGuid().ToString();
            var newDescription = System.Guid.NewGuid().ToString();
            var newLabels = System.Guid.NewGuid().ToString();

            a.Name = newName;
            a.Description = newDescription;
            a.Labels = newLabels;
            var updateResponse = await adminClient.PutAsJsonAsync($"{url}/{a.Id}", a);
            var updateResponseBody = await updateResponse.Content.ReadAsStringAsync();
            var b = JsonConvert.DeserializeObject<DetailedAmphora>(updateResponseBody);

            // Assert
            await AssertHttpSuccess(updateResponse);
            Assert.NotNull(b);
            Assert.NotNull(b.Id);
            Assert.Equal(a.Id, b.Id);
            Assert.Equal(a.Price, b.Price);
            // the updated properties
            Assert.Equal(newDescription, b.Description);
            Assert.Equal(newName, b.Name);
            Assert.Equal(newLabels, b.Labels);

            await DestroyAmphoraAsync(adminClient, b.Id);
            await DestroyOrganisationAsync(adminClient, adminOrg);
            await DestroyUserAsync(adminClient, adminUser);
        }

        [Fact]
        public async Task AmphoraWithLongName_CannotBeCreated()
        {
            var gen = new Helpers.RandomGenerator();
            var url = "/api/amphorae";
            // Arrange
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();
            var a = Helpers.EntityLibrary.GetAmphoraDto(adminOrg.Id, nameof(Post_CreatesAmphora_AsAdmin));
            a.Name = gen.RandomString(200);
            var s = JsonConvert.SerializeObject(a);
            var requestBody = new StringContent(s, Encoding.UTF8, "application/json");

            // Act
            adminClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var response = await adminClient.PostAsync(url, requestBody);

            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseBody);
            Assert.NotEmpty(responseBody);
        }
    }
}