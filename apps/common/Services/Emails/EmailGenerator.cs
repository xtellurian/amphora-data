using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Amphora.Common.Contracts;

namespace Amphora.Common.Services.Emails
{
    public class EmailGenerator : IEmailGenerator
    {
        private readonly IContentLoader contentLoader;
        private readonly IMarkdownToHtml markdownToHtml;
        private const string RootName = "wwwroot";
        private const string EmailDirectory = "emails";

        public EmailGenerator(IContentLoader contentLoader, IMarkdownToHtml markdownToHtml)
        {
            this.contentLoader = contentLoader;
            this.markdownToHtml = markdownToHtml;
        }

        public async Task<string> ContentFromMarkdownTemplateAsync(string templateName, Dictionary<string, string>? templateData)
        {
            if (contentLoader.FileExists(RootName, EmailDirectory, $"{templateName}.template.md"))
            {
                // file exists
                templateData ??= new Dictionary<string, string>();
                var path = contentLoader.GetFullyQualifiedPath(RootName, EmailDirectory, $"{templateName}.template.md");
                var contents = await contentLoader.ReadContentsAsStringAsync(path);
                var templated = ApplyTemplateData(contents, templateData);
                var html = markdownToHtml.ToHtml(templated);
                // now put html inside the html template
                var htmlTemplatePath = contentLoader.GetFullyQualifiedPath(RootName, EmailDirectory, "email.template.html");
                var htmlWrapper = await contentLoader.ReadContentsAsStringAsync(htmlTemplatePath);
                var fullContent = htmlWrapper.Replace("{{content}}", html);
                return fullContent;
            }
            else
            {
                throw new ArgumentException($"Email template {templateName} does not exist.");
            }
        }

        private string ApplyTemplateData(string template, Dictionary<string, string> data)
        {
            var sb = new StringBuilder(template);
            foreach (var kvp in data)
            {
                sb.Replace(kvp.Key, kvp.Value);
            }

            return sb.ToString();
        }
    }
}