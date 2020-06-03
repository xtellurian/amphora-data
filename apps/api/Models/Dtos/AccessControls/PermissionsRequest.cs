using System.Collections.Generic;

namespace Amphora.Api.Models.Dtos.AccessControls
{
    public class PermissionsRequest
    {
        public List<AccessLevelQuery> AccessQueries { get; set; }
    }
}