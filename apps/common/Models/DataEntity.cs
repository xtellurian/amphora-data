using Amphora.Common.Contracts;

namespace Amphora.Common.Models
{
    public abstract class DataEntity : Entity, IOrgEntity
    {
        public string OrgId { get; set; }
    }
}