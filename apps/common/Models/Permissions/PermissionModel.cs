using System.Collections.Generic;
using System.Linq;
using Amphora.Common.Contracts;
using Amphora.Common.Extensions;

namespace Amphora.Common.Models.Permissions
{
    public class PermissionModel : Entity, IEntity
    {
        public PermissionModel() { }
        public PermissionModel(string organisationId)
        {
            this.OrganisationId = organisationId;
            this.ResourceAuthorizations = new List<ResourceAuthorization>();
        }
        public List<ResourceAuthorization> ResourceAuthorizations { get; set; }
        public string PermissionCollectionId => this.OrganisationId;
        public override void SetIds() // org id and  permission id are the same!!
        {
            if (string.IsNullOrEmpty(this.OrganisationId))
            {
                throw new System.NullReferenceException($"SetIds() cannot be called in a {nameof(PermissionModel)} when OrganisationId null");
            }
            this.Id = this.OrganisationId.AsQualifiedId(typeof(PermissionModel));
        }

        public IEnumerable<ResourceAuthorization> GetAuthorizations(IEntity entity)
        {
            return this.ResourceAuthorizations.Where(a => string.Equals(a.TargetEntityId, entity.Id));
        }
    }
}