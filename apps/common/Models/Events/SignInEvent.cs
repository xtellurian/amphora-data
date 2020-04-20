using System.Collections.Generic;
using Amphora.Common.Contracts;

namespace Amphora.Common.Models.Events
{
    public class SignInEvent : EventBase, IEvent
    {
        public SignInEvent(IUser user, bool? isAcquiringToken = false)
        {
            var d = new EventDataDictionary($"{user.UserName} signed in");
            d.SetOrganisationId(user.OrganisationId);
            d.SetTriggeredByUsername(user.UserName);
            d.Set("IsAcquiringToken", isAcquiringToken.ToString());
            this.Data = d;
            Subject = $"/amphora/app/users/{user?.UserName}";
        }

        public string EventType => "AmphoraData.Users.SignIn";
        public IEventData Data { get; private set; }
        public override string Subject { get; set; }
    }
}