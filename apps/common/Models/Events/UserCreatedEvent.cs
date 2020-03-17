using Amphora.Common.Contracts;

namespace Amphora.Common.Models.Events
{
    public class UserCreatedEvent : EventBase, IEvent
    {
        public UserCreatedEvent(IUser user)
        {
            if (user is null)
            {
                throw new System.ArgumentNullException(nameof(user));
            }

            Subject = user.UserName;
            Data = new UserCreatedEventData(user.OrganisationId,
                                            user.Id,
                                            user.UserName);
        }

        public string EventType => "AmphoraData.Users.UserCreated";

        public IEventData Data { get; private set; }

        public string? Subject { get; private set; }

        private class UserCreatedEventData : IEventData
        {
            public UserCreatedEventData(string? organisationId,
                                        string? userId,
                                        string? triggeredByUserName)
            {
                OrganisationId = organisationId;
                UserId = userId;
                TriggeredByUserName = triggeredByUserName;
            }

            public string? AmphoraId { get; set; }
            public string? OrganisationId { get; set; }
            public string? TriggeredByUserName { get; set; }
            public string? UserId { get; set; }
        }
    }
}