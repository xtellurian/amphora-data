using System;
using Amphora.Common.Models.GitHub;
using Bogus;
using Xunit;

namespace Amphora.Tests.GitHub
{
    public class GitHubLinkingTests
    {
        private Faker faker = new Faker("en");
        private string CorrectlyFormattedIssueBody
        {
            get
            {
                return $"# {faker.Lorem.Sentence()}\n {faker.Lorem.Paragraphs(2)} {LinkInformation.LinkingSectionHeaderMd}\n {LinkInformation.IdLinePrefix}{Guid.NewGuid()}";
            }
        }

        [Fact]
        public void CanParseIssueBody_GetsAmphoraId()
        {
            var parseRes = LinkInformation.TryParse(CorrectlyFormattedIssueBody, out var body);
            Assert.True(parseRes);
            Assert.NotNull(body.AmphoraId);
        }

        [Fact]
        public void CanCreateIssueBody_ThatCanBeParsed()
        {
            var id = Guid.NewGuid().ToString();
            var bodyString = LinkInformation.Template(id);
            var parseRes = LinkInformation.TryParse(bodyString, out var body);
            Assert.True(parseRes);
            Assert.Equal(id, body.AmphoraId);
        }
    }
}