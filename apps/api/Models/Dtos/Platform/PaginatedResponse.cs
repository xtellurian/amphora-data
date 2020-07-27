using System.Collections.Generic;

namespace Amphora.Api.Models.Dtos.Platform
{
    public abstract class PaginatedResponse
    {
        public virtual IDictionary<string, string> ToRouteData()
        {
            return new Dictionary<string, string>
            {
                { nameof(Take), Take.ToString() },
                { nameof(Skip), Skip.ToString() }
            };
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