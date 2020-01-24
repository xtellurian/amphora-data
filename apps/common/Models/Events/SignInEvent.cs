using System;
using Amphora.Common.Contracts;

namespace Amphora.Common.Models.Events
{
    public class SignInEvent : EventBase, IEvent
    {
        public SignInEvent(IUser user)
        {
            Data = new { UserId = user.Id, orgId = user.OrganisationId };
            Subject = user.UserName;
        }

        public string EventType => "AmphoraData.Users.SignIn";

        public object Data { get; private set; }

        public string Subject { get; private set; }
    }
}