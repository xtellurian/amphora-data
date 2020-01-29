using Amphora.Common.Contracts;
using Amphora.Common.Models.Users;

namespace Amphora.Common.Models.Events
{
    public class UserCreatedEvent : EventBase, IEvent
    {
        public UserCreatedEvent(ApplicationUser user)
        {
            Subject = user.Email;
            Data = new UserCreatedEventData(user.OrganisationId,
                                            user.UserName,
                                            user.Email,
                                            user.Organisation?.Name,
                                            user.Id,
                                            user.FullName,
                                            user.About);
        }

        public string EventType => "AmphoraData.Users.UserCreated";

        public IEventData Data { get; private set; }

        public string Subject { get; private set; }

        private class UserCreatedEventData : IEventData
        {
            public UserCreatedEventData(string? organisationId,
                                        string? triggeredByUserName,
                                        string email,
                                        string? organisationName,
                                        string userId,
                                        string? fullName,
                                        string? about)
            {
                OrganisationId = organisationId;
                TriggeredByUserName = triggeredByUserName;
                Email = email;
                OrganisationName = organisationName;
                UserId = userId;
                FullName = fullName;
                About = about;
            }

            public string? AmphoraId { get; set; }
            public string? OrganisationId { get; set; }
            public string? TriggeredByUserName { get; set; }
            public string Email { get; set; }
            public string? OrganisationName { get; set; }
            public string UserId { get; set; }
            public string? FullName { get; set; }
            public string? About { get; set; }
        }
    }
}