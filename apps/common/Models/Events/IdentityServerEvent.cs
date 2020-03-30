using Amphora.Common.Contracts;

namespace Amphora.Common.Models.Events
{
    public class IdentityServerEvent : EventBase, IEvent
    {
        public IdentityServerEvent(string name, string message, string category)
        {
            Data = new IdentityServerEventData(name, message, category);
            Subject = category;
        }

        public string EventType => "AmphoraData.IdentityServer";

        public IEventData Data { get; }
        public string? Subject { get; }

        private class IdentityServerEventData : IEventData
        {
            public IdentityServerEventData(string? name, string? message, string? category)
            {
                Name = name;
                Message = message;
                Category = category;
            }

            public string? AmphoraId { get; set; } = null;
            public string? OrganisationId { get; set; } = null;
            public string? TriggeredByUserName { get; set; } = null;

            public string? Name { get; set; }
            public string? Message { get; set; }
            public string? Category { get; set; }
        }
    }
}