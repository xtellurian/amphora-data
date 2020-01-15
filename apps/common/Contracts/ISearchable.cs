namespace Amphora.Common.Contracts
{
    public interface ISearchable
    {
        bool? IsDeleted { get; set; }
        string? CreatedById { get; set; }
    }
}