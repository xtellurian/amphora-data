using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Amphora.GitHub.Models
{
    public class LinkInformation
    {
        public static string LinkingSectionHeaderMd => "⚠ Do not edit this section. It is required for amphoradata.com ➟ GitHub issue linking.";
        public static string IdLinePrefix => "* AmphoraId: ";
        private static string FullIdLinkRegex => @"(\* AmphoraId:[ ]*)" + GuidRegex; // matches "* AmphoraId: <GUID>"
        private static string GuidRegex => @"({{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}}{0,1})";
        private static string TemplateHeader => "<!-- Write here -->";
        private static string NewLines(int times = 1)
        {
            var sb = new StringBuilder();
            for (int i = 1; i <= times; i++)
            {
                sb.Append("%0A");
            }

            return sb.ToString();
        }

        public LinkInformation(string? amphoraId = null)
        {
            AmphoraId = amphoraId;
        }

        public string? AmphoraId { get; set; }
        public string? FullBody { get; set; }

        public static bool TryParse(string? fullBody, out LinkInformation? body)
        {
            try
            {
                var id = ExtractAmphoraId(fullBody);
                body = new LinkInformation(id);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                body = null;
                return false;
            }
        }

        public static string Template(string amphoraId)
        {
            return NewLines(1) + "---" + NewLines(2) + TemplateHeader + NewLines(2) + LinkingSectionHeaderMd + NewLines(1) + IdLinePrefix + amphoraId;
        }

        private static string? ExtractAmphoraId(string? fullBody)
        {
            if (fullBody == null) { return null; }
            var linkingSection = GetLinkingSection(fullBody) ?? "";
            if (Regex.IsMatch(linkingSection, FullIdLinkRegex))
            {
                var match = Regex.Match(linkingSection, FullIdLinkRegex);
                return ParseIdLine(match.Value);
            }

            return null;
        }

        private static string ParseIdLine(string idLine)
        {
            return idLine.Substring(IdLinePrefix.Length).Trim();
        }

        private static string? GetLinkingSection(string fullBody)
        {
            var linkingHeaderIndex = fullBody.LastIndexOf(LinkingSectionHeaderMd);
            if (linkingHeaderIndex >= 0)
            {
                var linkingSection = fullBody.Substring(linkingHeaderIndex);
                return linkingSection;
            }
            else
            {
                return null;
            }
        }
    }
}