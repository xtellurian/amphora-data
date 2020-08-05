using System.Collections.Generic;
using System.IO;
using System.Linq;
using Amphora.Api.Models.Versions;
using Amphora.Common.Contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Pages.Changelog
{
    public class IndexModel : PageModel
    {
        public IndexModel(IContentLoader contentLoader)
        {
            this.contentLoader = contentLoader;
        }

        public static string RootName = "wwwroot";
        public static string ChangelogsRelativePath = "docs/changelog";
        public IWebHostEnvironment Env { get; }
        public List<VersionFile> VersionDetails { get; private set; } = new List<VersionFile>();
        private IComparer<string> comparer = new VersionComparer();
        private readonly IContentLoader contentLoader;

        public IActionResult OnGetAsync()
        {
            var fullyQualifiedPaths = contentLoader.GetFullyQualifiedPaths(RootName, ChangelogsRelativePath);

            foreach (var f in fullyQualifiedPaths)
            {
                var version = Path.GetFileNameWithoutExtension(f).Replace('_', '.');
                var p = f.Substring(f.IndexOf(RootName) + RootName.Length);
                VersionDetails.Add(new VersionFile(version, p));
            }

            VersionDetails = VersionDetails.OrderByDescending(_ => _.VersionName, comparer).ToList();

            return Page();
        }
    }

    public class VersionComparer : IComparer<string>
    {
        public static bool IsNumeric(string value)
        {
            return int.TryParse(value, out _);
        }

        /// <inheritdoc />
        public int Compare(string s1, string s2)
        {
            var (s1Major, s1Minor, s1Patch) = GetSections(s1);
            var (s2Major, s2Minor, s2Patch) = GetSections(s2);

            var diff = (s1Major * 100) - (s2Major * 100)
                + (s1Minor * 10) - (s2Minor * 10)
                + s1Patch - s2Patch;

            return diff;
        }

        private (int major, int minor, int patch) GetSections(string version)
        {
            var array = version.Split('.');
            if (array.Length == 1 && IsNumeric(array[0]))
            {
                return (int.Parse(array[0]), -1, -1);
            }

            if (array.Length == 2 && IsNumeric(array[0]) && IsNumeric(array[1]))
            {
                return (int.Parse(array[0]), int.Parse(array[1]), -1);
            }
            else if (array.Length == 3 && IsNumeric(array[0]) && IsNumeric(array[1]) && IsNumeric(array[0]))
            {
                return (int.Parse(array[0]), int.Parse(array[1]), int.Parse(array[2]));
            }
            else
            {
                return (-1, -1, -1);
            }
        }
    }
}