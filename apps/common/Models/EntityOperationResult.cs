using System.Collections.Generic;
using System.Linq;
using Amphora.Common.Models.Users;

namespace Amphora.Common.Models
{
    public class EntityOperationResult<T> where T : class
    {
        public EntityOperationResult(bool succeeded = false)
        {
            Succeeded = succeeded;
        }

        public EntityOperationResult(string message)
        {
            Succeeded = false;
            this.Errors.Add(message);
        }

        public EntityOperationResult(ApplicationUser user, int? code = null, bool succeeded = false) : this(succeeded)
        {
            this.User = user;
            this.Code = code;
        }

        public EntityOperationResult(ApplicationUser user, string message, int? code = null, bool succeeded = false) : this(succeeded)
        {
            this.User = user;
            this.Code = code;
            this.Errors.Add(message);
        }

        public EntityOperationResult(ApplicationUser user, int? code, params string[] errors) : this(user, code)
        {
            this.Errors = errors.ToList();
            Succeeded = false;
        }

        public EntityOperationResult(ApplicationUser user, IEnumerable<string> errors, int? code = null) : this(user, code)
        {
            this.Errors = errors.ToList();
            Succeeded = false;
        }

        public EntityOperationResult(ApplicationUser user, T entity) : this(user)
        {
            Entity = entity;
            Succeeded = true;
            this.Code = 200;
        }

        public T? Entity { get; }
        public ApplicationUser? User { get; set; }
        public List<string> Errors { get; } = new List<string>();
        public int? Code { get; set; }
        public bool Succeeded { get; }
        public bool WasForbidden { get; set; }
        public string Message => string.Join(';', this.Errors ?? new List<string> { "An Unknown Error Occurred" });
    }
}