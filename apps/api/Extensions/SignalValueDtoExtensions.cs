using System.Collections.Generic;
using Amphora.Api.Models.Dtos.Amphorae;

namespace Amphora.Api.Extensions
{
    public static class SignalValueExtensions
    {
        public static Dictionary<string, object> ToObjectDictionary(this IEnumerable<SignalValueDto> signals)
        {
            var d = new Dictionary<string, object>();
            foreach (var s in signals)
            {
                if (s.IsString) d.Add(s.Property, s.StringValue);
                else if (s.IsNumeric) d.Add(s.Property, s.NumericValue);
                else if (s.IsDateTime) d.Add(s.Property, s.DateTimeValue);
            }
            return d;
        }
    }
}