using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amphora.Common.Contracts
{
    public interface IEmailGenerator
    {
        Task<string> ContentFromMarkdownTemplateAsync(string templateName, Dictionary<string, string>? templateData);
    }
}