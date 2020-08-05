using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Services.Emails;
using FluentAssertions;
using Moq;
using Xunit;

namespace Amphora.Tests.Unit.Services
{
    public class EmailGeneratorTests : UnitTestBase
    {
        [Fact]
        public async Task CanTemplate_BasicEmail()
        {
            var templateName = "template";
            var templateFullPath = "/path/to/template";
            var emailTemplatePath = "/path/to/emailtemplate";
            var templateData = new Dictionary<string, string>
            {
                { "replace_this", "with_this" },
                { "and_this", "with_something_else" },
            };
            var content = "This is some content that would be in a markdown file. " +
                "replace_this will be replaced. " +
                "and_this will be too, including and_this the second time";
            var htmlTemplate = "<div> {{content}} </div>";

            var mockContentLoader = new Mock<IContentLoader>();
            mockContentLoader.Setup(_ => _.FileExists(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);
            mockContentLoader.Setup(_ =>
                _.GetFullyQualifiedPath(It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.Is<string>(_ => _ == $"{templateName}.template.md")))
                .Returns(templateFullPath);
            mockContentLoader.Setup(_ =>
                _.GetFullyQualifiedPath(It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.Is<string>(_ => _ == "email.template.html")))
                .Returns(emailTemplatePath);
            mockContentLoader.Setup(_ => _.ReadContentsAsStringAsync(
                    It.Is<string>(v => v == templateFullPath),
                    It.IsAny<Encoding>()))
                .ReturnsAsync(content);
            mockContentLoader.Setup(_ => _.ReadContentsAsStringAsync(
                    It.Is<string>(v => v == emailTemplatePath),
                    It.IsAny<Encoding>()))
                .ReturnsAsync(htmlTemplate);

            var mockMarkdownToHtml = new Mock<IMarkdownToHtml>();
            mockMarkdownToHtml.Setup(_ => _.ToHtml(It.IsAny<string>())).Returns<string>((val) => val);
            var sut = new EmailGenerator(mockContentLoader.Object, mockMarkdownToHtml.Object);

            var result = await sut.ContentFromMarkdownTemplateAsync(templateName, templateData);
            foreach (var kvp in templateData)
            {
                result.Should().Contain(kvp.Value, "because it was templated");
                result.Should().NotContain(kvp.Key, "because these values should be remove");
            }
        }
    }
}