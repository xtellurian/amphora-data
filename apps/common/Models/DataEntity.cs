using Amphora.Common.Contracts;

namespace Amphora.Common.Models
{
    public abstract class OrgEntity : Entity, IOrgEntity
    {
        public string OrgId { get; set; }
    }
}