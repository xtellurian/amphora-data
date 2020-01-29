using System;
using Amphora.Common.Contracts;

namespace Amphora.Common.Models.Events
{
    public class SignInEvent : EventBase, IEvent
    {
        public SignInEvent(IUser user)
        {
            Data = new SignInEventData(user.OrganisationId, user.UserName);
            Subject = user.UserName;
        }

        public string EventType => "AmphoraData.Users.SignIn";

        public IEventData Data { get; private set; }

        public string Subject { get; private set; }
        private class SignInEventData : IEventData
        {
            public SignInEventData(string? organisationId, string? triggeredByUserName)
            {
                OrganisationId = organisationId;
                TriggeredByUserName = triggeredByUserName;
            }

            public string? AmphoraId { get; set; }
            public string? OrganisationId { get; set; }
            public string? TriggeredByUserName { get; set; }
        }
    }
}