using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Amphora.Common.Contracts
{
    public interface IContentLoader
    {
        bool FileExists(string filePath);
        bool FileExists(string root, string path);
        bool FileExists(string root, string folder, string fileName);
        string GetFullyQualifiedPath(string root, string path);
        string GetFullyQualifiedPath(string root, string folder, string fileName);
        IEnumerable<string> GetFullyQualifiedPaths(string root, string contentDirectoryPath);
        Task<string> ReadContentsAsStringAsync(string filePath, Encoding? encoding = null);
    }
}