using Amphora.Common.Contracts;

namespace Amphora.Common.Models.Events
{
    public class IdentityServerEvent : EventBase, IEvent
    {
        public IdentityServerEvent(string name, string message, string category)
        {
            Data = new IdentityServerEventData(name, message, category);
            Subject = $"amphora/identity/identityserver/{category}";
        }

        public string EventType => "AmphoraData.IdentityServer";

        public IEventData Data { get; }
        public override string Subject { get; set; }

        private class IdentityServerEventData : IEventData
        {
            public IdentityServerEventData(string? name, string? message, string? category)
            {
                FriendlyName = $"An Identity Server event occured.";
                Name = name;
                Message = message;
                Category = category;
            }

            public string? FriendlyName { get; set; }
            public string? AmphoraId { get; set; } = null;
            public string? OrganisationId { get; set; } = null;
            public string? TriggeredByUserName { get; set; } = null;

            public string? Name { get; set; }
            public string? Message { get; set; }
            public string? Category { get; set; }
        }
    }
}