using Amphora.Common.Contracts;

namespace Amphora.Common.Models
{
    public class User : OrgEntity, IOrgEntity
    {
        public string OrgId { get; set; }
        
    }
}