using System.Collections.Generic;
using System.Linq;

namespace Amphora.Api
{
    public static class ApiVersion
    {
        public const string HeaderName = "x-amphoradata-version";
        public static ApiVersionIdentifier CurrentVersion = new ApiVersionIdentifier(0, 2, 0, "dev1");

    }

    public class ApiVersionIdentifier
    {
        public int Major { get; private set; }
        public int Minor { get; private set; }
        public int Patch { get; private set; }
        public List<string> Suffixes { get; set; }

        public ApiVersionIdentifier(int major, int minor, int patch, params string[] suffixes)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            Suffixes = suffixes?.ToList();
        }
        public string ToSemver()
        {
            var version = $"{Major}.{Minor}.{Patch}";
            if(Suffixes != null && Suffixes.Count > 0)
            {
                version += "." + string.Join('.', Suffixes);
            }
            return version;
        }
    }
}
