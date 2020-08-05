using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public IEnumerable<string> GetFullyQualifiedPaths(string root, string contentDirectoryPath)
        {
            return Directory.EnumerateFiles(Path.Join(env.ContentRootPath, root, contentDirectoryPath));
        }

        public bool FileExists(string root, string folder, string fileName)
        {
            var p = Path.Join(root, folder, fileName);
            var fs = Directory.EnumerateFiles(Path.Join(root, folder)).ToList();
            return FileExists(p);
        }

        public bool FileExists(string root, string path)
        {
            return FileExists(Path.Join(root, path));
        }

        public bool FileExists(string filePath)
        {
            return FilePathExists(Path.Join(env.ContentRootPath, filePath));
        }

        private bool FilePathExists(string absolutePath)
        {
            return File.Exists(absolutePath);
        }

        public string GetFullyQualifiedPath(string root, string path)
        {
            return Path.Join(env.ContentRootPath, root, path);
        }

        public string GetFullyQualifiedPath(string root, string folder, string fileName)
        {
            return Path.Join(env.ContentRootPath, root, folder, fileName);
        }

        public async Task<string> ReadContentsAsStringAsync(string filePath, Encoding encoding = null)
        {
            if (!this.FilePathExists(filePath))
            {
                throw new ArgumentException($"File {filePath} does not exist");
            }

            encoding ??= Encoding.UTF8;
            return await File.ReadAllTextAsync(filePath, encoding);
        }
    }
}