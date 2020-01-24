namespace Amphora.Common.Models.Events
{
    public abstract class EventBase
    {
        public string Id { get; private set; } = System.Guid.NewGuid().ToString();
        public string DataVersion { get; private set; } = "0";
        public System.DateTime EventTime { get; private set; } = System.DateTime.UtcNow;
    }
}
