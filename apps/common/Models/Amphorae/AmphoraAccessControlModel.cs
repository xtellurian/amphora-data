using System.Collections.Generic;
using System.Collections.ObjectModel;
using Amphora.Common.Models.Permissions.Rules;

namespace Amphora.Common.Models.Amphorae
{
    public class AmphoraAccessControlModel : EntityBase
    {
        public AmphoraAccessControlModel()
        { }

        public AmphoraAccessControlModel(AmphoraModel amphora)
        {
            AmphoraId = amphora.Id;
            Amphora = amphora;
        }

        public string AmphoraId { get; set; } = null!;
        public AmphoraModel Amphora { get; set; } = null!;
        public ICollection<UserAccessRule> UserAccessRules { get; set; } = new Collection<UserAccessRule>();
        public ICollection<OrganisationAccessRule> OrganisationAccessRules { get; set; } = new Collection<OrganisationAccessRule>();
        public ICollection<AccessRule> Rules()
        {
            var all = new List<AccessRule>();
            all.AddRange(UserAccessRules);
            all.AddRange(OrganisationAccessRules);
            return all;
        }
    }
}