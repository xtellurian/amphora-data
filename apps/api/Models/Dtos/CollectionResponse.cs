using System.Collections.Generic;

namespace Amphora.Api.Models.Dtos
{
    public class CollectionResponse<T> : Response where T : IDto
    {
        public CollectionResponse(long count, IEnumerable<T> items)
        {
            Count = count;
            Items = items ?? new List<T>();
        }

        public long? Count { get; set; }
        public IEnumerable<T> Items { get; set; }
    }
}