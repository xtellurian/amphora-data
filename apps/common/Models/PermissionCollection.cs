using System.Collections.Generic;
using Amphora.Common.Contracts;
using Amphora.Common.Extensions;

namespace Amphora.Common.Models
{
    public class PermissionCollection : Entity, IEntity
    {
        public PermissionCollection(){}
        public PermissionCollection(string organisationId)
        {
            this.OrganisationId = organisationId;
            this.ResourceAuthorizations = new List<ResourceAuthorization>();
        }
        public List<ResourceAuthorization> ResourceAuthorizations { get; set; }
        public string PermissionCollectionId => this.OrganisationId;
        public override void SetIds() // org id and  permission id are the same!!
        {
            if(string.IsNullOrEmpty(this.OrganisationId))
            {
                throw new System.NullReferenceException($"SetIds() cannot be called in a {nameof(PermissionCollection)} when OrganisationId null");
            }
            this.Id = this.OrganisationId.AsQualifiedId(typeof(PermissionCollection));
        }
    }
}