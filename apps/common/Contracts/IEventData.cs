namespace Amphora.Common.Contracts
{
    public interface IEventData
    {
        /// <summary>
        /// The user friendly name of the event.
        /// </summary>
        string? FriendlyName { get; set; }
        string? AmphoraId { get; set; }
        string? OrganisationId { get; set; }
        string? TriggeredByUserName { get; set; }
    }
}