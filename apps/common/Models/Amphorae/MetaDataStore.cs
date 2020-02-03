using System.Collections.Generic;

namespace Amphora.Common.Models.Amphorae
{
    public class MetaDataStore
    {
        // outer dictionary contains the id of the subject
        // inner dictionary contains a set of key value pairs
        public IDictionary<string, IDictionary<string, string>> MetaData { get; set; } = new Dictionary<string, IDictionary<string, string>>();

        public void AddMetadata(string id, string key, string value)
        {
            CheckInputs(id, key, value);

            if (MetaData.TryGetValue(id, out var subject))
            {
                subject[key] = value;
            }
            else
            {
                MetaData[id] = new Dictionary<string, string>
                {
                    { key, value }
                };
            }
        }

        public void SetMetadata(string id, IDictionary<string, string> metadata)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new System.ArgumentException("Id cannot be null or empty", nameof(id));
            }

            if (metadata is null)
            {
                throw new System.ArgumentNullException(nameof(metadata));
            }

            MetaData[id] = metadata;
        }

        public IDictionary<string, string> GetMetadata(string id)
        {
            if (MetaData.ContainsKey(id))
            {
                return MetaData[id];
            }
            else
            {
                return new Dictionary<string, string>();
            }
        }

        private static void CheckInputs(string id, string key, string value)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new System.ArgumentException("Id cannot be null or empty", nameof(id));
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new System.ArgumentException("Name cannot be null or empty", nameof(key));
            }

            if (string.IsNullOrEmpty(value))
            {
                throw new System.ArgumentException("Value cannot be null or empty", nameof(value));
            }
        }
    }
}