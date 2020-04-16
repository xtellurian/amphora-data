using Amphora.Common.Contracts;

namespace Amphora.Common.Models.Events
{
    public class UserCreatedEvent : EventBase, IEvent
    {
        public UserCreatedEvent(string? email, string? phone, string? triggeredByUserName)
        {
            Subject = $"/amphora/app/users/{triggeredByUserName}";
            Data = new UserCreatedEventData(email, phone, triggeredByUserName);
        }

        public string EventType => "AmphoraData.Users.UserCreated";

        public IEventData Data { get; private set; }

        public override string Subject { get; set; }

        private class UserCreatedEventData : IEventData
        {
            public UserCreatedEventData(string? email, string? phone, string? triggeredByUserName)
            {
                FriendlyName = $"User created with email: {email} and phone {phone}";
                Phone = phone;
                Email = email;
                TriggeredByUserName = triggeredByUserName;
            }

            public string? FriendlyName { get; set; }
            public string? AmphoraId { get; set; } = null;
            public string? OrganisationId { get; set; } = null;
            public string? TriggeredByUserName { get; set; }
            public string? Email { get; set; }
            public string? Phone { get; set; }
        }
    }
}