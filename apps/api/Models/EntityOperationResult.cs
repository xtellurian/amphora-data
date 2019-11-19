using System.Collections.Generic;
using System.Linq;
using Amphora.Common.Models.Users;

namespace Amphora.Api.Models
{
    public class EntityOperationResult<T>
    {
        public EntityOperationResult(bool succeeded = false)
        {
            Succeeded = succeeded;
        }
        public EntityOperationResult(ApplicationUser user)
        {
            this.User = user;
        }
        public EntityOperationResult(ApplicationUser user, params string[] errors) : this(user)
        {
            this.Errors = errors.ToList();
            Succeeded = false;
        }
        public EntityOperationResult(ApplicationUser user, IEnumerable<string> errors) : this(user)
        {
            this.Errors = errors.ToList();
            Succeeded = false;
        }
        public EntityOperationResult(ApplicationUser user, T entity) : this(user)
        {
            Entity = entity;
            Succeeded = true;
        }
        public T Entity { get; }
        public ApplicationUser User { get; set; }
        public List<string> Errors { get; } = new List<string>();
        public bool Succeeded { get; }
        public bool WasForbidden { get; set; }
        public string Message => string.Join(';', this.Errors ?? new List<string> { "An Unknown Error Occurred" });
    }
}