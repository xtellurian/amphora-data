using System;
using System.Collections.Generic;
using Amphora.Common.Contracts;
using Azure.Storage.Blobs.Models;

namespace Amphora.Infrastructure.Stores.AzureStorage
{
    public class AzureBlobAmphoraFileReference : IAmphoraFileReference
    {
        private readonly BlobItem blob;

        public AzureBlobAmphoraFileReference(BlobItem blob, string name)
        {
            this.blob = blob;
            Name = name;
        }

        public string Name { get; set; }

        public DateTimeOffset? LastModified => blob.Properties.LastModified;
        public IDictionary<string, string> Metadata => blob.Metadata;
    }
}