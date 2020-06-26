using System.Collections.Generic;

namespace Amphora.Api.Models.Dtos.Amphorae.Files
{
    public class FileQueryOptions
    {
        public static FileQueryOptions Default => new FileQueryOptions
        {
            OrderBy = Alphabetical
        };

        public const string Alphabetical = nameof(Alphabetical);
        public const string LastModified = nameof(LastModified);
        public bool IsValid(out string message)
        {
            if (!string.IsNullOrEmpty(OrderBy) && (OrderBy != Alphabetical || OrderBy != LastModified))
            {
                message = $"OrderBy must be ${Alphabetical} or {LastModified}";
                return false;
            }

            message = null;
            return true;
        }

        /// <summary>
        /// Gets or sets how many files to return.
        /// Defaults to 64.
        /// </summary>
        public int Take { get; set; } = 64;

        /// <summary>
        /// Gets or sets how many files to skip before returning.
        /// Defaults to 0.
        /// </summary>
        public int Skip { get; set; } = 0;

        /// <summary>
        /// Gets or sets the the orderBy parameter.
        /// Options are Alphabetical or LastModified.
        /// </summary>
        public string OrderBy { get; set; }

        /// <summary>
        /// Gets or sets filters for all files with at least one of these attribute values.
        /// </summary>
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets or sets a value indicating whether whether all attributes are required to match.
        /// Defaults to false.
        /// </summary>
        public bool AllAttributes { get; set; } = false;

        /// <summary>
        /// Gets or sets a prefix filter for all file names.
        /// Is case sensitive.
        /// </summary>
        public string Prefix { get; set; }
    }
}