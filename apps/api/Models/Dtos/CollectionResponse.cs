using System.Collections.Generic;
using System.Linq;

namespace Amphora.Api.Models.Dtos
{
    public class CollectionResponse<T> : Response where T : IDto
    {
        public CollectionResponse()
        { }

        public CollectionResponse(IEnumerable<T> items)
        {
            Items = items ?? new List<T>();
            Count ??= items.Count();
        }

        public CollectionResponse(long count, IEnumerable<T> items) : this(items)
        {
            Count = count;
        }

        public long? Count { get; set; }
        public IEnumerable<T> Items { get; set; }
    }
}