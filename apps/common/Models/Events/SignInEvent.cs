using System;
using Amphora.Common.Contracts;

namespace Amphora.Common.Models.Events
{
    public class SignInEvent : EventBase, IEvent
    {
        public SignInEvent(IUser user, bool? isAcquiringToken = false)
        {
            Data = new SignInEventData(user.OrganisationId, user.UserName, isAcquiringToken);
            Subject = user.UserName;
        }

        public string EventType => "AmphoraData.Users.SignIn";

        public IEventData Data { get; private set; }

        public string Subject { get; private set; }
        private class SignInEventData : IEventData
        {
            public SignInEventData(string? organisationId, string? triggeredByUserName, bool? isAcquiringToken)
            {
                OrganisationId = organisationId;
                TriggeredByUserName = triggeredByUserName;
                IsAcquiringToken = isAcquiringToken;
            }

            public string? AmphoraId { get; set; } = null;
            public string? OrganisationId { get; set; }
            public string? TriggeredByUserName { get; set; }
            public bool? IsAcquiringToken { get; set; }
        }
    }
}