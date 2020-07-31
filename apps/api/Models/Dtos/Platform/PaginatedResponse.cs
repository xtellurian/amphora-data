using System.Collections.Generic;

namespace Amphora.Api.Models.Dtos.Platform
{
    public abstract class PaginatedResponse
    {
        protected void AddIfNotNull(IDictionary<string, string> dict, string name, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                dict.Add(name, value);
            }
        }

        protected void AddIfNotNull(IDictionary<string, string> dict, string name, int? value)
        {
            if (value.HasValue)
            {
                dict.Add(name, value.Value.ToString());
            }
        }

        protected void AddIfNotNull(IDictionary<string, string> dict, string name, double? value)
        {
            if (value.HasValue)
            {
                dict.Add(name, value.Value.ToString());
            }
        }

        public virtual IDictionary<string, string> ToRouteData()
        {
            var routeData = new Dictionary<string, string>();
            AddIfNotNull(routeData, nameof(Take), Take);
            AddIfNotNull(routeData, nameof(Skip), Skip);
            return routeData;
        }

        /// <summary>
        /// Gets or sets how many files to return.
        /// Defaults to 64.
        /// </summary>
        public int? Take { get; set; } = 64;

        /// <summary>
        /// Gets or sets how many files to skip before returning.
        /// Defaults to 0.
        /// </summary>
        public int? Skip { get; set; } = 0;
    }
}