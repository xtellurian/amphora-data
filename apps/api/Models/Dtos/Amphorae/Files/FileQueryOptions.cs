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

        public string OrderBy { get; set; }
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
    }
}