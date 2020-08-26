namespace Amphora.Api.Models.Dtos.Search
{
    public class SearchResponse<T> : CollectionResponse<T> where T : IDto
    { }
}