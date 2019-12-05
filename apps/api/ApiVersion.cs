using System.Collections.Generic;
using System.Linq;

namespace Amphora.Api
{
    public static class ApiVersion
    {
        public const string HeaderName = "x-amphoradata-version";
        public static ApiVersionIdentifier CurrentVersion
            = new ApiVersionIdentifier()
            {
                Major = 0,
                Minor = 3,
                Patch = 1
            };
    }

    public class ApiVersionIdentifier
    {
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Patch { get; set; }
        public List<string> Suffixes { get; set; }
        public ApiVersionIdentifier() { }
        public ApiVersionIdentifier(int major, int minor, int patch, params string[] suffixes)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            Suffixes = suffixes?.ToList();
        }
        public ApiVersionIdentifier(int major, int minor, int patch, IEnumerable<string> suffixes)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            Suffixes = suffixes?.ToList();
        }
        public string ToSemver()
        {
            var version = $"{Major}.{Minor}.{Patch}";
            if (Suffixes != null && Suffixes.Count > 0)
            {
                version += "." + string.Join('.', Suffixes);
            }
            return version;
        }

        public static ApiVersionIdentifier FromSemver(string semver)
        {
            var lines = semver.Split('.');
            if (lines.Count() < 3) throw new System.ArgumentException("SemVer must have at least 3 sections");

            var identifier = new ApiVersionIdentifier(int.Parse(lines[0]), int.Parse(lines[1]), int.Parse(lines[2]), lines.Skip(3));
            return identifier;
        }
    }
}
