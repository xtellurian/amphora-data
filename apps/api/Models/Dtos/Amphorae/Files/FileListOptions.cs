// these are used for GET requests
namespace Amphora.Api.Models.Dtos.Amphorae.Files
{
    public class FileListOptions
    {
        public const string Alphabetical = nameof(Alphabetical);
        public const string LastModified = nameof(LastModified);

        public static FileQueryOptions Default => new FileQueryOptions
        {
            OrderBy = Alphabetical
        };

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