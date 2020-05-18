namespace Amphora.Common.Contracts
{
    public interface ITelemetry : IEventPublisher
    {
        void TrackMetricValue(string metric, double? value);
        void TrackMetricValue(string metric, string dimension, double? value);
    }
}