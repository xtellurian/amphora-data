using Amphora.Common.Contracts;

namespace Amphora.Common.Models.Events
{
    public class UserCreatedEvent : EventBase, IEvent
    {
        public UserCreatedEvent(string? email, string? phone, string? triggeredByUserName)
        {
            Subject = triggeredByUserName;
            Data = new UserCreatedEventData(email, phone, triggeredByUserName);
        }

        public string EventType => "AmphoraData.Users.UserCreated";

        public IEventData Data { get; private set; }

        public string? Subject { get; private set; }

        private class UserCreatedEventData : IEventData
        {
            public UserCreatedEventData(string? email, string? phone, string? triggeredByUserName)
            {
                Phone = phone;
                Email = email;
                TriggeredByUserName = triggeredByUserName;
            }

            public string? AmphoraId { get; set; } = null;
            public string? OrganisationId { get; set; } = null;
            public string? TriggeredByUserName { get; set; }
            public string? Email { get; set; }
            public string? Phone { get; set; }
        }
    }
}