using System.Collections.Generic;

namespace Amphora.Api.Models.Dtos.Amphorae.Files
{
    public class FileQueryOptions : FileListOptions
    {
        /// <summary>
        /// Gets or sets the attribute filters.
        /// </summary>
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets or sets a value indicating whether whether all attributes are required to match.
        /// Defaults to false.
        /// </summary>
        public bool AllAttributes { get; set; } = false;
    }
}