using System.Collections.Generic;
using System.Linq;

namespace Amphora.Api.Models
{
    public class EntityOperationResult<T>
    {
        public EntityOperationResult()
        {
            Succeeded = true;
            this.Errors = new List<string>();
        }
        public EntityOperationResult(params string[] errors)
        {
            this.Errors = errors.ToList();
            Succeeded = false;
        }
        public EntityOperationResult( IEnumerable<string> errors)
        {
            this.Errors = errors.ToList();
            Succeeded = false;
        }
        public EntityOperationResult(T entity)
        {
            Entity = entity;
            Succeeded = true;
        }
        public T Entity { get; }
        public List<string> Errors { get; }
        public bool Succeeded { get; }
        public bool WasForbidden {get; set; }
        public string Message => string.Join(';', this.Errors);
    }
}