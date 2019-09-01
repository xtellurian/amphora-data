using Amphora.Common.Contracts;

namespace Amphora.Common.Models
{
    public class Organisation: Entity, IEntity
    {
        // needs nothing as of now
        public string InviteCode {get; set; }
        public string Name {get; set; }
    }
}