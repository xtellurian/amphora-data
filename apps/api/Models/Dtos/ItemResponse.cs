using Newtonsoft.Json;

namespace Amphora.Api.Models.Dtos
{
    public class ItemResponse<T> where T : IDto
    {
        [JsonConstructor]
        public ItemResponse(T item)
        {
            Item = item;
        }

        public ItemResponse(T item, string message) : this(item)
        {
            Message = message;
        }

        public string Message { get; set; }
        public T Item { get; set; }
    }
}