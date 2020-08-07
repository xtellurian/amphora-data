using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Microsoft.AspNetCore.Hosting;

namespace Amphora.SharedUI.Services.Content
{
    public class StaticContentLoader : IContentLoader
    {
        private readonly IWebHostEnvironment env;

        public StaticContentLoader(IWebHostEnvironment env)
        {
            this.env = env;
        }

        private string AddContentRoot(string path)
        {
            var root = env.ContentRootPath;
            if (path.StartsWith(root))
            {
                return path;
            }
            else
            {
                return Path.Join(root, path);
            }
        }

        public IEnumerable<string> GetFullyQualifiedPaths(string root, string contentDirectoryPath)
        {
            root = AddContentRoot(root);
            return Directory.EnumerateFiles(Path.Join(root, contentDirectoryPath));
        }

        public bool FileExists(string root, string folder, string fileName)
        {
            root = AddContentRoot(root);
            return File.Exists(Path.Join(root, folder, fileName));
        }

        public bool FileExists(string root, string path)
        {
            return FileExists(Path.Join(root, path));
        }

        public bool FileExists(string filePath)
        {
            filePath = AddContentRoot(filePath);
            return File.Exists(filePath);
        }

        public string GetFullyQualifiedPath(string root, string path)
        {
            root = AddContentRoot(root);
            return Path.Join(root, path);
        }

        public string GetFullyQualifiedPath(string root, string folder, string fileName)
        {
            root = AddContentRoot(root);
            return Path.Join(root, folder, fileName);
        }

        public async Task<string> ReadContentsAsStringAsync(string filePath, Encoding encoding = null)
        {
            filePath = AddContentRoot(filePath);
            if (!File.Exists(filePath))
            {
                throw new ArgumentException($"File {filePath} does not exist");
            }

            encoding ??= Encoding.UTF8;
            return await File.ReadAllTextAsync(filePath, encoding);
        }
    }
}