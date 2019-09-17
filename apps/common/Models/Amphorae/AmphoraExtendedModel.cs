
using System.Collections.Generic;
using Amphora.Common.Models.UserData;

namespace Amphora.Common.Models.Amphorae
{
    public class AmphoraExtendedModel : AmphoraModel
    {
        public string Description { get; set; }
        public GeoLocation GeoLocation { get; set; }
    }
}