using Amphora.Common.Contracts;

namespace Amphora.Common.Models.Events
{
    public class UserCreatedEvent : EventBase, IEvent
    {
        public UserCreatedEvent(string? email, string? phone, string? triggeredByUserName)
        {
            Subject = $"/amphora/app/users/{triggeredByUserName}";
            var d = new EventDataDictionary($"User created for email: {email}");
            d.Set("Phone", phone);
            d.Set("Email", email);
            d.SetTriggeredByUsername(triggeredByUserName);
            this.Data = d;
        }

        public string EventType => "AmphoraData.Users.UserCreated";
        public IEventData Data { get; private set; }
        public override string Subject { get; set; }
    }
}