namespace Amphora.Common.Contracts
{
    public interface ISearchable
    {
        string Name { get; set; }
        bool? IsDeleted { get; set; }
        string? CreatedById { get; set; }
    }
}