using System.Collections.Generic;
using System.Linq;
using Amphora.Common.Models;
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
    }
}