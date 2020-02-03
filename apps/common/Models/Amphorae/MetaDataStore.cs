using System.Collections.Generic;

namespace Amphora.Common.Models.Amphorae
{
    public class MetaDataStore
    {
        public IDictionary<string, string> MetaData { get; set; } = new Dictionary<string, string>();
    }
}