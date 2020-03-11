namespace Amphora.Common.Contracts
{
    public interface IEvent
    {
        string Id { get; }
        string EventType { get; }
        IEventData Data { get; }
        System.DateTime EventTime { get; }
        string? Subject { get; }
        string DataVersion { get; }
    }
}