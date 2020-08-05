using Amphora.Common.Contracts;
using Westwind.AspNetCore.Markdown;

namespace Amphora.SharedUI.Services.Content
{
    public class MarkdownToHtml : IMarkdownToHtml
    {
        public string ToHtml(string markdown)
        {
            var html = Markdown.Parse(markdown);
            return html.Replace(@"\n", "<br/>");
        }
    }
}