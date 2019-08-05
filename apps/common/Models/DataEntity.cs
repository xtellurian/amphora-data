using Amphora.Common.Contracts;

namespace Amphora.Common.Models
{
    public abstract class DataEntity : Entity, IDataEntity
    {
        public string OrgId { get; set; }
    }
}