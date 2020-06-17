using System.Collections.Generic;

namespace Amphora.Api.Models.Dtos.Applications
{
    public abstract class AppLocationBase
    {
        /// <summary>
        /// Gets or sets the expected origin from a XMLHttpRequest
        /// Must not end in '/'.
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// Gets or sets allowed redirects after login, relative to Origin.
        /// Must begin with a '/'.
        /// </summary>
        public IList<string> AllowedRedirectPaths { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the allowed redirect after logout.
        /// Must be an absolute url.
        /// </summary>
        public IList<string> PostLogoutRedirects { get; set; } = new List<string>();
    }
}