using System;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Microsoft.Azure.Storage.Blob;

namespace Amphora.Infrastructure.Stores.AzureStorage
{
    public class AzureBlobAmphoraFileReference : IAmphoraFileReference
    {
        private readonly CloudBlob blob;

        public AzureBlobAmphoraFileReference(CloudBlob blob, string name)
        {
            this.blob = blob;
            Name = name;
        }

        public async Task LoadAttributesAsync()
        {
            await blob.FetchAttributesAsync();
        }

        public string Name { get; set; }

        public DateTimeOffset? LastModified => blob.Properties.LastModified;
    }
}