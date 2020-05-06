using System.Collections.Generic;
using Amphora.Common.Contracts;

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

        public EntityOperationResult(IUser user, bool succeeded) : this(succeeded)
        {
            this.User = user;
        }

        public EntityOperationResult(IUser user, string message) : this(false)
        {
            this.User = user;
            this.Errors.Add(message);
        }

        public EntityOperationResult(IUser user, params string[] messages) : this(false)
        {
            this.User = user;
            this.Errors.AddRange(messages);
        }

        public EntityOperationResult(IUser user, T entity) : this(user, entity != null)
        {
            Entity = entity;
            this.Code = entity != null ? 200 : 404;
        }

        public T? Entity { get; }
        public IUser? User { get; set; }
        public List<string> Errors { get; } = new List<string>();
        public int? Code { get; set; }
        public bool Succeeded { get; }
        public bool Failed => !Succeeded;
        public bool WasForbidden { get; set; }
        public string Message => string.Join(';', this.Errors ?? new List<string> { "An Unknown Error Occurred" });
    }
}