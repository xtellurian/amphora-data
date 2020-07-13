// these are used for GET requests
using Amphora.Api.Models.Dtos.Platform;

namespace Amphora.Api.Models.Dtos.Amphorae.Files
{
    public class FileListOptions : PaginatedResponse
    {
        public const string Alphabetical = nameof(Alphabetical);
        public const string LastModified = nameof(LastModified);

        public static FileQueryOptions Default => new FileQueryOptions
        {
            OrderBy = Alphabetical
        };

        /// <summary>
        /// Gets or sets the the orderBy parameter.
        /// Options are Alphabetical or LastModified.
        /// </summary>
        public string OrderBy { get; set; }

        /// <summary>
        /// Gets or sets a prefix filter for all file names.
        /// Is case sensitive.
        /// </summary>
        public string Prefix { get; set; }

        public virtual bool IsValid(out string message)
        {
            if (!string.IsNullOrEmpty(OrderBy) && (OrderBy != Alphabetical || OrderBy != LastModified))
            {
                message = $"OrderBy must be ${Alphabetical} or {LastModified}";
                return false;
            }

            message = null;
            return true;
        }
    }
}