namespace Amphora.Common.Contracts
{
    public interface IEvent
    {
        string Id { get; }
        string EventType { get; }
        IEventData Data { get; }
        System.DateTime EventTime { get; }
        /// <summary>
        /// or your events that make it easy for subscribers to know whether they're interested in the event.
        /// </summary>
        string Subject { get; }
        string DataVersion { get; }
    }
}