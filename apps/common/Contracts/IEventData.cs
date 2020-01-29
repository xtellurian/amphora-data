namespace Amphora.Common.Contracts
{
    public interface IEventData
    {
        string? AmphoraId { get; set; }
        string? OrganisationId { get; set; }
        string? TriggeredByUserName { get; set; }
    }
}