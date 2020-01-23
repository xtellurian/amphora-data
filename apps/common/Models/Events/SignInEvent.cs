using System;
using Amphora.Common.Contracts;

namespace Amphora.Common.Models.Events
{
    public class SignInEvent : IEvent
    {
        public SignInEvent(IUser user)
        {
            Data = new { UserId = user.Id, orgId = user.OrganisationId };
            Subject = user.UserName;
        }

        public string Id { get; private set; } = System.Guid.NewGuid().ToString();

        public string EventType => "AmphoraData.Users.SignIn";

        public object Data { get; private set; }

        public DateTime EventTime { get; private set; } = DateTime.UtcNow;

        public string Subject { get; private set; }
        public string DataVersion => "0";
    }
}