using System.Collections.Generic;
using System.Linq;
using Amphora.Common.Models.Amphorae;

namespace Amphora.Common.Extensions
{
    public static class EntityExtensions
    {
        public static string ToLabelString(this IEnumerable<Label> labels)
        {
            if (labels == null) { return ""; }
            else
            {
                return string.Join(',', labels.Select(_ => _.Name));
            }
        }

        public static Dictionary<string, AttributeStore> ToMetadataDictionary(this ICollection<SignalV2> v2Signals)
        {
            if (v2Signals == null) { return new Dictionary<string, AttributeStore>(); }
            else
            {
                var meta = new Dictionary<string, AttributeStore>();
                foreach (var s in v2Signals)
                {
                    meta[s.Id] = s.Attributes;
                }

                return meta;
            }
        }

        public static Dictionary<string, string> ToChildDictionary(this Dictionary<string, KeyValuePair<string, string>> kvps)
        {
            var dic = new Dictionary<string, string>();
            foreach (var kvp in kvps)
            {
                if (!dic.ContainsKey(kvp.Value.Key))
                {
                    dic.Add(kvp.Value.Key, kvp.Value.Value);
                }
                else
                {
                    throw new System.ArgumentException("Duplicate key");
                }
            }

            return dic;
        }
    }
}
