using Amphora.Common.Contracts;

namespace Amphora.Common.Models
{
    public abstract class Entity : IEntity
    {
        public string Id { get; set; }
    }
}