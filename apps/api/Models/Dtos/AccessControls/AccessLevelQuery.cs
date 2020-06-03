using System.ComponentModel.DataAnnotations;

namespace Amphora.Api.Models.Dtos.AccessControls
{
    public class AccessLevelQuery
    {
        /// <summary>
        /// Gets or sets The id of the Amphora you are checking.
        /// </summary>
        [StringLength(50, MinimumLength = 5)]
        public string AmphoraId { get; set; }

        /// <summary>
        /// Gets or sets the access level that will be checked.
        /// Ranges from 0 (none) to 256 (Administer).
        /// </summary>
        /// <remarks>
        /// None = 0,
        /// Read = 16,
        /// Purchase = 24,
        /// ReadContents = 32,
        /// CreateEntities = 48,
        /// WriteContents = 64,
        /// Update = 128,
        /// Administer = 256.
        /// </remarks>
        public int AccessLevel { get; set; }
    }

    public class AccessLevelResponse : AccessLevelQuery
    {
        public AccessLevelResponse(AccessLevelQuery query, bool isAuthorized)
        {
            this.AmphoraId = query?.AmphoraId;
            this.AccessLevel = query?.AccessLevel ?? -1;
            IsAuthorized = isAuthorized;
        }

        /// <summary>
        /// Gets or sets a value indicating whether is authorized at that level.
        /// </summary>
        public bool IsAuthorized { get; set; }
    }
}