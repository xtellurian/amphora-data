using System.Collections.Generic;

namespace Amphora.Api.Models.Dtos.AccessControls
{
    public class PermissionsResponse
    {
        public PermissionsResponse(List<AccessLevelResponse> accessResponses)
        {
            AccessResponses = accessResponses;
        }

        public List<AccessLevelResponse> AccessResponses { get; set; }
    }
}