using System.Collections.Generic;

namespace Amphora.Common.Models.Amphorae
{
    public class AttributeStore
    {
        public AttributeStore()
        {
        }

        public AttributeStore(IDictionary<string, string> metaData)
        {
            MetaData = metaData;
        }

        // outer dictionary contains the id of the subject
        // inner dictionary contains a set of key value pairs
        public IDictionary<string, string> MetaData { get; set; } = new Dictionary<string, string>();

        public void AddMetadata(string key, string value)
        {
            CheckInputs(key, value);
            this.MetaData[key] = value;
        }

        private static void CheckInputs(string key, string value)
        {
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